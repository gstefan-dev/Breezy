using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class CreateSoundPackCommandHandler
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateSoundPackCommandHandler> _logger;

    public CreateSoundPackCommandHandler(ISoundPackRepository soundPackRepository, 
        IUserRepository userRepository, 
        ILogger<CreateSoundPackCommandHandler> logger)
    {
        _soundPackRepository = soundPackRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<SoundPackResponse> Handle(CreateSoundPackCommand command)
    {
        _logger.LogInformation("Creating SoundPack {PackName} for UserId: {UserId}", command.PackName, command.UserId);



        var user = await _userRepository.GetUserByIdAsync(command.UserId);

        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", command.UserId);
            throw new DomainException("User not found");
        }

        var soundPack = SoundPack.Create(user.Id, user.UserName, command.PackName, command.Description, command.Genre,
            command.Mood, command.PackCategory);

        await _soundPackRepository.AddAsync(soundPack);
        
        _logger.LogInformation("SoundPack {PackName} created successfully", soundPack.PackName);
        return new SoundPackResponse(soundPack.Id, soundPack.PackName, soundPack.OwnerName, soundPack.Genre,
             soundPack.PackCategory, soundPack.CreatedAt);

    }
}