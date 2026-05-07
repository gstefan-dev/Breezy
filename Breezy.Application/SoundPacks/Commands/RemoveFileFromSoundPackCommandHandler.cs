using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class RemoveFileFromSoundPackCommandHandler
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RemoveFileFromSoundPackCommandHandler> _logger;

    public RemoveFileFromSoundPackCommandHandler(ISoundPackRepository soundPackRepository, IUserRepository userRepository,
        ILogger<RemoveFileFromSoundPackCommandHandler> logger)
    {
        _soundPackRepository = soundPackRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SoundPackFileResponse> Handle(RemoveFileFromSoundPackCommand command)
    {
        _logger.LogInformation("Removing file {FileId} from SoundPack {SoundPackId}", command.FileId, command.SoundPackId);


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

        if (soundPack.OwnerUserId != command.UserId)
        {
            _logger.LogWarning("User {UserId} is not owner of SoundPack {SoundPackId}", command.UserId, command.SoundPackId);
            throw new DomainException("User is not the soundpack's Owner");
        }

        var file = soundPack.Files.FirstOrDefault(f => f.Id == command.FileId);
        if (file == null)
        {
            _logger.LogWarning("File {FileId} not found in SoundPack", command.FileId);
            throw new DomainException("File not found");
        }
        
        soundPack.RemoveFile(command.FileId);
        user.FreeStorage(file.FileSize);

        await _soundPackRepository.UpdateAsync(soundPack);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("File {FileId} removed successfully from SoundPack {SoundPackId}", command.FileId, command.SoundPackId);
        return new SoundPackFileResponse(file.Id, file.SoundPackId, file.FileName, file.BlobUrl,
            file.Category.ToString());

    }
}