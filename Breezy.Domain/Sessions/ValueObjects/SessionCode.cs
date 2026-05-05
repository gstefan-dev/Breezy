using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Shared;

namespace Breezy.Domain.Sessions.ValueObjects;

public class SessionCode : ValueObject
{
    public string Value { get; }
    private static readonly char[] _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
    private SessionCode() { }

    public SessionCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidSessionCodeException("Session code cannot be empty", value ?? "");
        string cleanCode = value.Trim().ToUpper();

        if (cleanCode.Length < 4 || cleanCode.Length > 10)
            throw new InvalidSessionCodeException(
                $"Session code must be 4-10 characters long. Provided: {cleanCode.Length}", cleanCode);
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(cleanCode, @"^[A-Z0-9]+$"))
            throw new InvalidSessionCodeException(
                "Session code must contain only uppercase letters and numbers", cleanCode);
        Value = cleanCode;

    }
    
    public static SessionCode Generate()
    {
        var code = new char[6];
        for (int i = 0; i < 6; i++)
            code[i] = _chars[Random.Shared.Next(_chars.Length)];
        return new SessionCode(new string(code));
    }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}