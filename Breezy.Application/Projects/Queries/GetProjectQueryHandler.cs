using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Queries;

public class GetProjectQueryHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<GetProjectQueryHandler> _logger;

    public GetProjectQueryHandler(IProjectRepository projectRepository, ILogger<GetProjectQueryHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<ProjectResponse> Handle(GetProjectQuery query)
    {
        _logger.LogInformation("Getting project {ProjectId}", query.ProjectId);


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
        
        _logger.LogInformation("Project {ProjectId} retrieved successfully", query.ProjectId);

        return new ProjectResponse(project.Id, project.OwnerUserId, project.ProjectName, project.CreatedAt);
    }
}