using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Exceptions;

public class InvalidParticipantException : DomainException
{
    public Guid UserId { get; }

    public InvalidParticipantException(string message, Guid userId = default)
        : base(message)
    {
        UserId = userId;
    }
}