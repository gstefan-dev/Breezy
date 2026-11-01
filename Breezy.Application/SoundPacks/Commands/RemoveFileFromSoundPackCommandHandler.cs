using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class RemoveFileFromSoundPackCommandHandler : IRequestHandler<RemoveFileFromSoundPackCommand, SoundPackFileResponse>
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

    public async Task<SoundPackFileResponse> Handle(RemoveFileFromSoundPackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing file {FileId} from SoundPack {SoundPackId}", request.FileId, request.SoundPackId);


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

        if (soundPack.OwnerUserId != request.UserId)
        {
            _logger.LogWarning("User {UserId} is not owner of SoundPack {SoundPackId}", request.UserId, request.SoundPackId);
            throw new DomainException("User is not the soundpack's Owner");
        }

        var file = soundPack.Files.FirstOrDefault(f => f.Id == request.FileId);
        if (file == null)
        {
            _logger.LogWarning("File {FileId} not found in SoundPack", request.FileId);
            throw new DomainException("File not found");
        }
        
        soundPack.RemoveFile(request.FileId);
        user.FreeStorage(file.FileSize);

        await _soundPackRepository.UpdateAsync(soundPack);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation("File {FileId} removed successfully from SoundPack {SoundPackId}", request.FileId, request.SoundPackId);
        return new SoundPackFileResponse(file.Id, file.SoundPackId, file.FileName, file.BlobUrl,
            file.Category.ToString(), file.DurationInSeconds, file.Bpm, file.Key, file.FileSize);

    }
}