using Breezy.Domain.Sessions.ValueObjects;

namespace Breezy.Application.Sessions.Commands;

public record CreateSessionRequest(string SessionName, int MaxParticipants, SessionVisibility Visibility);
