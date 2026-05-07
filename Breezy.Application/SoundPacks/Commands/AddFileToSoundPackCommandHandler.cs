using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class AddFileToSoundPackCommandHandler
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

    public async Task<SoundPackFileResponse> Handle(AddFileToSoundPackCommand command)
    {
        _logger.LogInformation("Adding file {FileName} to SoundPack {SoundPackId}", command.FileName, command.SoundPackId);

        var soundPack = await _soundPackRepository.GetByIdAsync(command.SoundPackId);

        if (soundPack ==  null)
        {
            _logger.LogWarning("SoundPack {SoundPackId} not found", command.SoundPackId);
            throw new DomainException("SoundPack dont exist");
        }
        
        var user = await _userRepository.GetUserByIdAsync(command.UserId);

        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", command.UserId);
            throw new DomainException("User not found");
        }

        if (soundPack.OwnerUserId != user.Id)
        {
            _logger.LogWarning("User {UserId} is not owner of SoundPack {SoundPackId}", command.UserId, command.SoundPackId);
            throw new DomainException("User is not the soundpack's Owner");
        }

        if (!user.HasAvailableStorage(command.FileSize))
        {
            _logger.LogWarning("User dont have storage");
            throw new DomainException("User dont have storage");
        }
        
        var blobName = $"{command.UserId}/{Guid.NewGuid()}_{command.FileName}";
        await _azureBlobService.UploadAsync(blobName, command.FileContent, command.FileType);

        var url = await _azureBlobService.GetUrlAsync(blobName);

        if (url == null)
        {
            _logger.LogWarning("Url doesnt exist");
            throw new DomainException("Url doesnt exist");
        }

        var soundPackFile = SoundPackFile.Create(soundPack.Id, command.FileName, command.SoundCategory, url,
            command.DurationSeconds, command.BPM, command.Key, command.FileSize);
        
        soundPack.AddFile(soundPackFile);
        user.UseStorage(command.FileSize);
        
        await _soundPackRepository.UpdateAsync(soundPack);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("File {FileName} added successfully to SoundPack {SoundPackId}", command.FileName, command.SoundPackId);

        return new SoundPackFileResponse(soundPackFile.Id, soundPackFile.SoundPackId, soundPackFile.FileName,
            soundPackFile.BlobUrl, soundPackFile.Category.ToString());

    }
}