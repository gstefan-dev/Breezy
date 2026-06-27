using Breezy.Application.Interfaces;
using Breezy.Application.Projects.DTOs;
using Breezy.Domain.Projects.Entities;
using Breezy.Domain.Projects.ValueObjects;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.Projects.Commands;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectResponse>
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

    public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating project {ProjectName} for OwnerId: {OwnerId}", request.ProjectName, request.OwnerId);

        var user = await _userRepository.GetUserByIdAsync(request.OwnerId);
        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", request.OwnerId);
            throw new DomainException("User not found");
        }
        
        if (!user.HasAvailableStorage(request.FileSize))
        {
            _logger.LogWarning("User {OwnerId} has insufficient storage", request.OwnerId);
            throw new DomainException("User dont have storage");
        }
        
        var blobName = $"{request.OwnerId}/{Guid.NewGuid()}_{request.ProjectName}";
        await _azureBlobService.UploadAsync(blobName, request.FileContent, request.FileType);

        var url = await _azureBlobService.GetUrlAsync(blobName);

        if (url == null)
        {
            _logger.LogWarning("Url is null");
            throw new DomainException("Url is null");
        }
        
        var projectHash = new ProjectHash(request.Hash);
        
        var project = DawProject.Create(request.OwnerId, request.ProjectName, request.LocalPath, projectHash, url);
        await _projectRepository.AddAsync(project);
        
        user.UseStorage(request.FileSize);
        await _userRepository.UpdateAsync(user);
          
        _logger.LogInformation("Project {ProjectName} created successfully for OwnerId: {OwnerId}", 
            request.ProjectName, request.OwnerId);
        
        return new ProjectResponse(project.Id, project.OwnerUserId, project.ProjectName, project.CreatedAt);
    }
}