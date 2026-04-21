using Breezy.Domain.Shared;

namespace Breezy.Domain.Users.ValueObjects;

public class Email : ValueObject
{
    private string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email cannot be empty");
        }
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(value.Trim().ToLower(), @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$"))
            throw new DomainException("Invalid email format");
        
        return new Email(value.Trim().ToLower());
    }
    
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public override string ToString() => Value;
    
    public static implicit operator string(Email email) => email.Value;
}