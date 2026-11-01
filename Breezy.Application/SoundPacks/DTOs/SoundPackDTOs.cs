namespace Breezy.Application.SoundPacks.DTOs;

public record SoundPackResponse(Guid Id, string SoundPackName, string OwnerName, string Category,string PackCategory, DateTime CreatedAt);

public record SoundPackFileResponse(Guid Id, Guid SoundPackId, string FileName, string BlobUrl, string Category,
    int DurationInSeconds, int? Bpm, string? Key, long FileSize);

public record SoundPackDetailResponse(Guid Id, string SoundPackName, string OwnerName, string Description,
    string Genre, string Mood, string PackCategory, DateTime CreatedAt, IEnumerable<SoundPackFileResponse> Files);
