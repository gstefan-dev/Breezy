using Breezy.Application.Sessions.DTOs;
using Breezy.Domain.Sessions.ValueObjects;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record CreateCollaborationSessionCommand(Guid HostId, string SessionName, 
    int MaxParticipants, SessionVisibility Visibility) : IRequest<SessionResponse>;