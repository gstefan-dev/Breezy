using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Application.Sessions.Commands;
using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Commands;

public class CreateProjectVersionCommandHandler : IRequestHandler<CreateProjectVersionCommand, ProjectVersionResponse>
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

    public async Task<ProjectVersionResponse> Handle(CreateProjectVersionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new version for ProjectId: {ProjectId}", request.ProjectId);

        var project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project is null)
        {
            _logger.LogWarning("Project {ProjectId} not found", request.ProjectId);
            throw new DomainException("Project doesnt exist");
        }

        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", request.UserId);
            throw new DomainException("User doesnt exist");
        }

        if (!user.HasAvailableStorage(request.FileSize))
        {
            _logger.LogWarning("User {OwnerId} has insufficient storage", user.Id);
            throw new DomainException("User dont have storage");
        }
        
        var blobName = $"{request.UserId}/{Guid.NewGuid()}_{project.ProjectName}";

        await _azureBlobService.UploadAsync(blobName, request.FileContent, request.FileType);

        var url = await _azureBlobService.GetUrlAsync(blobName);
        if (url == null)
        {
            _logger.LogWarning("Failed to get URL for blob {BlobName}", blobName);
            throw new DomainException("Failed to get file URL");
        }
        
        var projectHash = new ProjectHash(request.NewHash);

        project.CreateNewVersionIfChanged(projectHash, url);

        await _projectRepository.UpdateAsync(project);
        user.UseStorage(request.FileSize);

        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("New version is created successfully");

        var version = project.GetLatestVersion();
        return new ProjectVersionResponse(version.Id, version.ProjectId, version.VersionNumber, version.Changelog?.Changes, version.CreatedAt);
    }
    
}