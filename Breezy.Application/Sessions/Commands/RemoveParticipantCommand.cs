using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record RemoveParticipantCommand(Guid OwnerId, string SessionCode, Guid ParticipantUserId) : IRequest;