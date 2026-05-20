using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record ReopenSessionCommand(Guid UserId, string SessionCode) : IRequest<SessionResponse>;