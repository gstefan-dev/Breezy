using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record CreateCollaborationSessionCommand(Guid HostId, string SessionName, 
    int MaxParticipants) : IRequest<SessionResponse>;