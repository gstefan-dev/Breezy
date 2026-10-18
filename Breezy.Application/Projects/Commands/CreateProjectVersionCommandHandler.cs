using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
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

    public async Task<ProjectVersionResponse> Handle(CreateProjectVersionCommand request,
        CancellationToken cancellationToken)
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

        if (project.OwnerUserId != user.Id)
        {
            _logger.LogWarning("User {UserId} is not the owner of project {ProjectId}", user.Id, project.Id);
            throw new DomainException("User is not the project owner");
        }

        if (project.CurrentHash.Value == request.NewHash)
        {
            _logger.LogWarning("Project {ProjectId} has not changed since the last version", project.Id);
            throw new DomainException("File has not changed since the last version");
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

        project.CreateNewVersionIfChanged(new ProjectHash(request.NewHash), url);
        var version = project.GetLatestVersion();
        await _projectRepository.AddVersionAsync(version);

        user.UseStorage(request.FileSize);
        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Version {VersionNumber} created for project {ProjectId}", version.VersionNumber,
            project.Id);

        return new ProjectVersionResponse(version.Id, version.ProjectId, version.VersionNumber,
            version.Changelog?.Changes, version.CreatedAt);
    }
}

