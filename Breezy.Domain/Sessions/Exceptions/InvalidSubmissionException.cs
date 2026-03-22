using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Exceptions;

public class InvalidSubmissionException : DomainException
{
    public string FieldName { get; }

    public InvalidSubmissionException(string message, string fieldName = "")
        : base(message)
    {
        FieldName = fieldName;
    }
}