namespace Breezy.Application.SoundPacks.Commands;

public record CreateSoundPackRequest(string PackName, string Description, string Mood, string Genre, string PackCategory);