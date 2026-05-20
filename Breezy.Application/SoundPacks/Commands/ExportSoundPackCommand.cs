using Breezy.Application.SoundPacks.DTOs;
using Breezy.Domain.SoundPacks.ValueObjects;
using MediatR;

namespace Breezy.Application.SoundPacks.Commands;

public record ExportSoundPackCommand(
    Guid SoundPackId,
    Guid UserId,
    ExportFormatEnum ExportFormat) : IRequest<SoundPackResponse>;
