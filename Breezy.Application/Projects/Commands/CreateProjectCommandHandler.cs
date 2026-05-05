using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Domain.Projects.Entities;
using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Commands;

public class CreateProjectCommandHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(IProjectRepository projectRepository, IUserRepository userRepository,IAzureBlobService azureBlobService, ILogger<CreateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _azureBlobService = azureBlobService;
        _logger = logger;
    }

    public async Task<ProjectResponse> Handle(CreateProjectCommand command)
    {
        _logger.LogInformation("Creating project {ProjectName} for OwnerId: {OwnerId}", command.ProjectName, command.OwnerId);

        var user = await _userRepository.GetUserByIdAsync(command.OwnerId);
        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", command.OwnerId);
            throw new DomainException("User not found");
        }

        if (!user.HasAvailableStorage(command.FileSize))
        {
            _logger.LogWarning("User {OwnerId} has insufficient storage", command.OwnerId);
            throw new DomainException("User dont have storage");
        }
        
        var blobName = $"{command.OwnerId}/{Guid.NewGuid()}_{command.ProjectName}";
        await _azureBlobService.UploadAsync(blobName, command.FileContent, command.FileType);

        var url = await _azureBlobService.GetUrlAsync(blobName);

        if (url == null)
        {
            _logger.LogWarning("Url is null");
            throw new DomainException("Url is null");
        }
        
        var projectHash = new ProjectHash(command.Hash);

        var project = DawProject.Create(command.OwnerId, command.ProjectName, command.LocalPath, projectHash, url);
        await _projectRepository.AddAsync(project);
        
        user.UseStorage(command.FileSize);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("Project {ProjectName} created successfully for OwnerId: {OwnerId}", 
            command.ProjectName, command.OwnerId);

        return new ProjectResponse(project.Id, project.OwnerUserId, project.ProjectName, project.CreatedAt);
    }
}