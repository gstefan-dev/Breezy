namespace Breezy.Application.Sessions.Commands;

public record CloseSessionCommand(Guid UserId, string SessionCode);