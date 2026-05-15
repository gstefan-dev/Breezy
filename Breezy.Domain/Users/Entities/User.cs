using Breezy.Domain.Shared;
using Breezy.Domain.Users.ValueObjects;

namespace Breezy.Domain.Users.Entities;

public class User : Entity
{
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string UserName { get; private set; }
    public UserRole Role { get; private set; }
    public SubscriptionPlan Plan { get; private set; }
    public StorageQuota Quota { get; private set; }
    public long UsedStorageBytes { get; private set; }

    private User() { }
    private User(Guid id,
        Email email,
        string passwordHash,
        string userName,
        UserRole role,
        SubscriptionPlan plan,
        StorageQuota quota,
        long usedStorageBytes) : base(id)

    {
        Email = email;
        PasswordHash = passwordHash;
        UserName = userName;
        Role = role;
        Plan = plan;
        Quota = quota;
        UsedStorageBytes = usedStorageBytes;
    }

    public static User Create(Email email, string passwordHash, string userName)
    {
        if (email is null)
            throw new DomainException("Email cannot be null");
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("PasswordHash cannot be empty");

        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("UserName cannot be empty");

        return new User(
            Guid.NewGuid(),
            email,
            passwordHash,
            userName,
            UserRole.User,
            SubscriptionPlan.Free,
            StorageQuota.ForFree(),
            0L);
    }

    public void UpdateSubscription(SubscriptionPlan plan)
    {
        Plan = plan;
        Quota = plan switch
        {
            SubscriptionPlan.Free => StorageQuota.ForFree(),
            SubscriptionPlan.Premium => StorageQuota.ForPremium(),
            SubscriptionPlan.Pro => StorageQuota.ForPro(),
            _ => throw new DomainException("Unknown subscription plan")
        };
        
        UpdateTimestamp();

    }

    public void UseStorage(long bytes)
    {
        UsedStorageBytes += bytes;
    }

    public void FreeStorage(long bytes)
    {
        UsedStorageBytes = Math.Max(0, UsedStorageBytes - bytes);
    }

    public bool HasAvailableStorage(long bytes)
    {
        return UsedStorageBytes + bytes <= Quota.Bytes;
    }
}