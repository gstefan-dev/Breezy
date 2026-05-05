using Breezy.Domain.Shared;

namespace Breezy.Domain.Users.ValueObjects;

public class StorageQuota : ValueObject
{
    public long Bytes { get; }

    private StorageQuota(long bytes)
    {
        Bytes = bytes;
    }

    public static StorageQuota ForFree() => new StorageQuota(1L * 1024 * 1024 * 1024);
    
    public static StorageQuota ForPremium() => new StorageQuota(10L * 1024 * 1024 * 1024);

    public static StorageQuota ForPro() => new StorageQuota(100L * 1024 * 1024 * 1024);

    public bool HasSpaceFor(long fileSize)
    {
        return fileSize <= Bytes;
    }


    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Bytes.ToString();
    }
}