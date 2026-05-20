using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class AddFileToSoundPackCommandHandler : IRequestHandler<AddFileToSoundPackCommand, SoundPackFileResponse>
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly IAzureBlobService _azureBlobService;
    private readonly ILogger<AddFileToSoundPackCommandHandler> _logger;
    private readonly IUserRepository _userRepository;

    public AddFileToSoundPackCommandHandler(ISoundPackRepository soundPackRepository, IAzureBlobService azureBlobService, 
        ILogger<AddFileToSoundPackCommandHandler> logger, IUserRepository userRepository)
    {
        _soundPackRepository = soundPackRepository;
        _azureBlobService = azureBlobService;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<SoundPackFileResponse> Handle(AddFileToSoundPackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding file {FileName} to SoundPack {SoundPackId}", request.FileName, request.SoundPackId);

        var soundPack = await _soundPackRepository.GetByIdAsync(request.SoundPackId);

        if (soundPack ==  null)
        {
            _logger.LogWarning("SoundPack {SoundPackId} not found", request.SoundPackId);
            throw new DomainException("SoundPack dont exist");
        }
        
        var user = await _userRepository.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", request.UserId);
            throw new DomainException("User not found");
        }

        if (soundPack.OwnerUserId != user.Id)
        {
            _logger.LogWarning("User {UserId} is not owner of SoundPack {SoundPackId}", request.UserId, request.SoundPackId);
            throw new DomainException("User is not the soundpack's Owner");
        }

        if (!user.HasAvailableStorage(request.FileSize))
        {
            _logger.LogWarning("User dont have storage");
            throw new DomainException("User dont have storage");
        }
        
        var blobName = $"{request.UserId}/{Guid.NewGuid()}_{request.FileName}";
        await _azureBlobService.UploadAsync(blobName, request.FileContent, request.FileType);

        var url = await _azureBlobService.GetUrlAsync(blobName);

        if (url == null)
        {
            _logger.LogWarning("Url doesnt exist");
            throw new DomainException("Url doesnt exist");
        }

        var soundPackFile = SoundPackFile.Create(soundPack.Id, request.FileName, request.SoundCategory, url,
            request.DurationSeconds, request.BPM, request.Key, request.FileSize);
        
        soundPack.AddFile(soundPackFile);
        user.UseStorage(request.FileSize);
        
        await _soundPackRepository.UpdateAsync(soundPack);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("File {FileName} added successfully to SoundPack {SoundPackId}", request.FileName, request.SoundPackId);

        return new SoundPackFileResponse(soundPackFile.Id, soundPackFile.SoundPackId, soundPackFile.FileName,
            soundPackFile.BlobUrl, soundPackFile.Category.ToString());

    }
}