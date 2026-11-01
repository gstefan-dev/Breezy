using Breezy.Application.Interfaces;
using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Breezy.Application.SoundPacks.Queries;

public class GetSoundPackQueryHandler : IRequestHandler<GetSoundPackQuery, SoundPackDetailResponse>
{
    private readonly ISoundPackRepository _soundPackRepository;
    private readonly ILogger<GetSoundPackQueryHandler> _logger;

    public GetSoundPackQueryHandler(ISoundPackRepository soundPackRepository, ILogger<GetSoundPackQueryHandler> logger)
    {
        _soundPackRepository = soundPackRepository;
        _logger = logger;
    }

    public async Task<SoundPackDetailResponse> Handle(GetSoundPackQuery query, CancellationToken cancellationToken)
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


        return new SoundPackDetailResponse(
            soundPack.Id, soundPack.PackName, soundPack.OwnerName, soundPack.Description,
            soundPack.Genre, soundPack.Mood, soundPack.PackCategory, soundPack.CreatedAt,
            soundPack.Files.Select(f => new SoundPackFileResponse(
                f.Id, f.SoundPackId, f.FileName, f.BlobUrl, f.Category.ToString(),
                f.DurationInSeconds, f.Bpm, f.Key, f.FileSize)));

    }
}