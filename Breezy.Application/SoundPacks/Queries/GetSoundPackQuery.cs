using Breezy.Application.SoundPacks.DTOs;
using MediatR;

namespace Breezy.Application.SoundPacks.Queries;

public record GetSoundPackQuery(Guid SoundPackId, Guid UserId) : IRequest<SoundPackResponse>;