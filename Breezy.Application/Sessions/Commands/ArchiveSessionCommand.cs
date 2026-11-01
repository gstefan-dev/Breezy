using MediatR;

namespace Breezy.Application.Sessions.Commands;

public record ArchiveSessionCommand(Guid UserId, string SessionCode) : IRequest;