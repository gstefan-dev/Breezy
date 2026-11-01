using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record RestoreSessionCommand(Guid UserId, string SessionCode) : IRequest;