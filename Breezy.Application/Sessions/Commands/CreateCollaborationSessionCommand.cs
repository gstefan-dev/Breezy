namespace Breezy.Application.Sessions.Commands;

public record CreateCollaborationSessionCommand(Guid HostId, string SessionName, int MaxParticipants);