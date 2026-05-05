namespace Breezy.Application.Sessions.Commands;

public record ReopenSessionCommand(Guid UserId, string SessionCode);