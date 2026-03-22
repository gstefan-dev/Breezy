using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Exceptions;

public class SessionDomainException : DomainException
{
    public Guid SessionId { get; }
    public Guid? UserId { get; }

    public SessionDomainException(string message, Guid sessionId = default, Guid? userId = null)
        : base(message)
    {
        SessionId = sessionId;
        UserId = userId;
    }
}