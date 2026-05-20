using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Queries;

public class GetProjectVersionQueryHandler : IRequestHandler<GetProjectVersionQuery, ProjectVersionResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetProjectVersionQueryHandler> _logger;

    public GetProjectVersionQueryHandler(IProjectRepository projectRepository, ILogger<GetProjectVersionQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<ProjectVersionResponse> Handle(GetProjectVersionQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting latest version for ProjectId: {ProjectId}", query.ProjectId);

        var project = await _projectRepository.GetByIdAsync(query.ProjectId);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", query.ProjectId);
            throw new DomainException("Project not found");
        }

        if (project.OwnerUserId != query.UserId)
        {
            _logger.LogWarning("User {UserId} unauthorized to access project {ProjectId}", 
                query.UserId, query.ProjectId);
            throw new DomainException("You don't have access to this project");
        }

        var version = project.GetLatestVersion();
        
        _logger.LogInformation("Getting latest version for project {ProjectId}", query.ProjectId);
        return new ProjectVersionResponse(version.Id, version.ProjectId, version.VersionNumber,
            version.Changelog?.Changes, version.CreatedAt);
    }
}