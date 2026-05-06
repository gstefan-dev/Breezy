using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Commands;

public class AddChangelogToVersionCommandHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<AddChangelogToVersionCommandHandler> _logger;

    public AddChangelogToVersionCommandHandler(IProjectRepository projectRepository,
        ILogger<AddChangelogToVersionCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<ProjectVersionResponse> Handle(AddChangelogToVersionCommand command)
    {
        _logger.LogInformation("Adding changelog to version {VersionNumber} of project {ProjectId}", 
            command.VersionNumber, command.ProjectId);

        var project = await _projectRepository.GetByIdAsync(command.ProjectId);

        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found", command.ProjectId);
            throw new DomainException("Project not found");
        }

        project.AddChangeLogToVersion(command.VersionNumber, command.Changelog);
        await _projectRepository.UpdateAsync(project);

        _logger.LogInformation("Version was changed successfully");

        var version = project.GetVersion(command.VersionNumber);
        return new ProjectVersionResponse(version.Id, version.ProjectId, version.VersionNumber,
            version.Changelog?.Changes,
            version.CreatedAt);
    }
}