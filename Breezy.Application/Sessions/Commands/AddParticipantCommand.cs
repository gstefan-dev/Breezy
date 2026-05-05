namespace Breezy.Application.Sessions.Commands;

public record AddParticipantCommand(Guid UserId, string SessionCode);
