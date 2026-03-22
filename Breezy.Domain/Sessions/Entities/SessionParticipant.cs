using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Entities;

public class SessionParticipant : Entity
{
    public Guid SessionId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserName { get; private set; }
    public DateTime JoinedAt { get; private set; }
    
    private SessionParticipant() {}

    public SessionParticipant(
        Guid sessionId,
        Guid userId,
        string userName
        ) : base(Guid.NewGuid())
    {
        if (sessionId == Guid.Empty)
            throw new InvalidParticipantException("SessionId cannot be empty");

        if (userId == Guid.Empty)
            throw new InvalidParticipantException("UserId cannot be empty", userId);

        if (string.IsNullOrWhiteSpace(userName))
            throw new InvalidParticipantException("UserName cannot be empty", userId);

        SessionId = sessionId;
        UserId = userId;
        UserName = userName.Trim();
        JoinedAt = DateTime.UtcNow;

    }
}