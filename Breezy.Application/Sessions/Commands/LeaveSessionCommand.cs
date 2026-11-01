using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record LeaveSessionCommand(Guid UserId, string SessionCode) : IRequest;
