namespace Breezy.Application.SoundPacks.Commands;

public record RemoveFileFromSoundPackCommand(Guid SoundPackId, Guid UserId, Guid FileId, long FileSize);