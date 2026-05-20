using Breezy.Application.SoundPacks.DTOs;
using MediatR;

namespace Breezy.Application.SoundPacks.Commands;

public record RemoveFileFromSoundPackCommand(Guid SoundPackId, Guid UserId,
    Guid FileId, long FileSize) : IRequest<SoundPackFileResponse>;