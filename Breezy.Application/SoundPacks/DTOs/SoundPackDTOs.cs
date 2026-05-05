namespace Breezy.Application.SoundPacks.DTOs;

public record SoundPackResponse(Guid Id, string SoundPackName, string OwnerName, string Category, DateTime CreatedAt);
public record SoundPackFileResponse(Guid Id, Guid SoundPackId, string FileName, string FileUrl, string Category);