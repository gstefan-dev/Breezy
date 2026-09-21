namespace Breezy.Application.Sessions.Commands;

public record CreateSessionRequest(string SessionName, int MaxParticipants);
