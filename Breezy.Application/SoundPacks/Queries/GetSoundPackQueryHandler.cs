using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Queries;

public class GetSoundPackQueryHandler : IRequestHandler<GetSoundPackQuery, SoundPackResponse>
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly ILogger<GetSoundPackQueryHandler> _logger;

    public GetSoundPackQueryHandler(ISoundPackRepository soundPackRepository, ILogger<GetSoundPackQueryHandler> logger)
    {
        _soundPackRepository = soundPackRepository;
        _logger = logger;
    }

    public async Task<SoundPackResponse> Handle(GetSoundPackQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting SoundPack {SoundPackId} for UserId: {UserId}", query.SoundPackId, query.UserId);


        var soundPack = await _soundPackRepository.GetByIdAsync(query.SoundPackId);
        if (soundPack ==  null)
        {
            _logger.LogWarning("SoundPack {SoundPackId} not found", query.SoundPackId);
            throw new DomainException("SoundPack dont exist");
        }

        if (soundPack.OwnerUserId != query.UserId)
        {
            _logger.LogWarning("User {UserId} is not owner of SoundPack {SoundPackId}", query.UserId, query.SoundPackId);
            throw new DomainException("User is not the soundpack's Owner");
        }
        
        _logger.LogInformation("SoundPack {SoundPackId} retrieved successfully", query.SoundPackId);


        return new SoundPackResponse(soundPack.Id, soundPack.PackName, soundPack.OwnerName,
            soundPack.Genre, soundPack.PackCategory, soundPack.CreatedAt);

    }
}