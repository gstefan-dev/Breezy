using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using Breezy.Domain.SoundPacks.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Commands;

public class CreateSoundPackCommandHandler : IRequestHandler<CreateSoundPackCommand, SoundPackResponse>
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

    public async Task<SoundPackResponse> Handle(CreateSoundPackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating SoundPack {PackName} for UserId: {UserId}", request.PackName, request.UserId);



        var user = await _userRepository.GetUserByIdAsync(request.UserId);

        if (user == null)
        {
            _logger.LogWarning("User with {OwnerId} not found", request.UserId);
            throw new DomainException("User not found");
        }

        var soundPack = SoundPack.Create(user.Id, user.UserName, request.PackName, request.Description, request.Genre,
            request.Mood, request.PackCategory);

        await _soundPackRepository.AddAsync(soundPack);
        
        _logger.LogInformation("SoundPack {PackName} created successfully", soundPack.PackName);
        return new SoundPackResponse(soundPack.Id, soundPack.PackName, soundPack.OwnerName, soundPack.Genre,
             soundPack.PackCategory, soundPack.CreatedAt);

    }
}