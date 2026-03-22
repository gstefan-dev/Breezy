using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.Exceptions;

public class InvalidSessionCodeException : DomainException
{
    public string ProvidedValue { get; }

    public InvalidSessionCodeException(string message, string providedValue = "")
        : base(message)
    {
        ProvidedValue = providedValue;
    }
}