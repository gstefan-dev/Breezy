using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Queries;

public class GetProjectVersionsQueryHandler : IRequestHandler<GetProjectVersionsQuery, IEnumerable<ProjectVersionResponse>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetProjectVersionsQueryHandler> _logger;

    public GetProjectVersionsQueryHandler(IProjectRepository projectRepository, ILogger<GetProjectVersionsQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ProjectVersionResponse>> Handle(GetProjectVersionsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all versions for ProjectId: {ProjectId}", query.ProjectId);


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
        
        _logger.LogInformation("Found {Count} versions for project {ProjectId}", 
            project.Versions.Count, query.ProjectId);
        return project.Versions.Select(version => new ProjectVersionResponse(
            version.Id,
            version.ProjectId,
            version.VersionNumber,
            version.Changelog?.Changes,
            version.CreatedAt));
    }

  
}