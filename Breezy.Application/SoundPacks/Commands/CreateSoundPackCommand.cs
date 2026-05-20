using Breezy.Application.SoundPacks.DTOs;
using MediatR;

namespace Breezy.Application.SoundPacks.Commands;

public record CreateSoundPackCommand(Guid UserId, string PackName, string Description,
    string Mood, string Genre, string PackCategory) : IRequest<SoundPackResponse>;