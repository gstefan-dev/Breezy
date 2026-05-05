using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Application.Sessions.Commands;
using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Commands;

public class CreateProjectVersionCommandHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<CreateProjectVersionCommandHandler> _logger;

    public CreateProjectVersionCommandHandler(IProjectRepository projectRepository, IUserRepository userRepositor, 
        IAzureBlobService azureBlobService,
        ILogger<CreateProjectVersionCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepositor;
        _azureBlobService = azureBlobService;
        _logger = logger;
    }

    public async Task<ProjectVersionResponse> Handle(CreateProjectVersionCommand command)
    {
        _logger.LogInformation("Creating new version for ProjectId: {ProjectId}", command.ProjectId);

        var project = await _projectRepository.GetByIdAsync(command.ProjectId);
        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", command.ProjectId);
            throw new DomainException("Project doesnt exist");
        }

        var user = await _userRepository.GetUserByIdAsync(command.UserId);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            throw new DomainException("User doesnt exist");
        }

        if (!user.HasAvailableStorage(command.FileSize))
        {
            _logger.LogWarning("User {OwnerId} has insufficient storage", user.Id);
            throw new DomainException("User dont have storage");
        }
        
        var blobName = $"{command.UserId}/{Guid.NewGuid()}_{project.ProjectName}";

        await _azureBlobService.UploadAsync(blobName, command.FileContent, command.FileType);

        var url = await _azureBlobService.GetUrlAsync(blobName);
        if (url == null)
        {
            _logger.LogWarning("Failed to get URL for blob {BlobName}", blobName);
            throw new DomainException("Failed to get file URL");
        }
        
        var projectHash = new ProjectHash(command.NewHash);

        project.CreateNewVersionIfChanged(projectHash, url);

        await _projectRepository.UpdateAsync(project);
        user.UseStorage(command.FileSize);

        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("New version is created successfully");

        var version = project.GetLatestVersion();
        return new ProjectVersionResponse(version.Id, version.ProjectId, version.VersionNumber, version.Changelog?.Changes, version.CreatedAt);
    }
    
}