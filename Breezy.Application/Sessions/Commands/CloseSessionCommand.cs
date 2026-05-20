using Breezy.Application.Sessions.DTOs;
using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record CloseSessionCommand(Guid UserId, string SessionCode) : IRequest<SessionResponse>;