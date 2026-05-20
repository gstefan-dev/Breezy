using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Queries;

public class GetUserSoundPacksQueryHandler : IRequestHandler<GetUserSoundPacksQuery, IEnumerable<SoundPackResponse>>
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly ILogger<GetUserSoundPacksQueryHandler> _logger;

    public GetUserSoundPacksQueryHandler(ISoundPackRepository soundPackRepository, ILogger<GetUserSoundPacksQueryHandler> logger)
    {
        _soundPackRepository = soundPackRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<SoundPackResponse>> Handle(GetUserSoundPacksQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting SoundPacks for UserId: {UserId}", query.UserId);

        var soundPacks = await _soundPackRepository.GetByUserIdAsync(query.UserId);
        
        _logger.LogInformation("Found {Count} SoundPacks for UserId: {UserId}", soundPacks.Count(), query.UserId);
        
        return soundPacks.Select(soundPack => new SoundPackResponse(
            soundPack.Id, soundPack.PackName,
            soundPack.OwnerName, soundPack.Genre,
            soundPack.PackCategory, soundPack.CreatedAt));
    }
}