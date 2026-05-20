using Breezy.Application.SoundPacks.DTOs;
using MediatR;

namespace Breezy.Application.SoundPacks.Queries;

public record GetUserSoundPacksQuery(Guid UserId) : IRequest<IEnumerable<SoundPackResponse>>;
