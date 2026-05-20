using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record AddParticipantCommand(Guid UserId, string SessionCode) : IRequest<ParticipantResponse>;
