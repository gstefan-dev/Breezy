using Breezy.Domain.Shared;

namespace Breezy.Domain.Projects.ValueObjects;

public class ProjectHash : ValueObject
{
    public string Value { get; }
    
    private ProjectHash() {}

    public ProjectHash(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Project hash cannot be empty");

        string cleanHash = value.Trim().ToUpper();

        if (cleanHash.Length != 64)
            throw new DomainException("Project must be exactly 64 characters (SHA 256)");

        if (!System.Text.RegularExpressions.Regex.IsMatch(cleanHash, @"^[A-F0-9]{64}$"))
            throw new DomainException("Project hash must contain only hexadecimal characters (0-9, A-F)");

        Value = cleanHash;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

}