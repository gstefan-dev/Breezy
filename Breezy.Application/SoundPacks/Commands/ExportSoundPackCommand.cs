using Breezy.Domain.SoundPacks.ValueObjects;

namespace Breezy.Application.SoundPacks.Commands;

public record ExportSoundPackCommand(Guid SoundPackId, Guid UserId, ExportFormatEnum ExportFormat);
