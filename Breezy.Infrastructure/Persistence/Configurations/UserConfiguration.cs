using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasConversion(
                email => email.ToString(),
                value => Email.From(value))
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.Plan)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.UsedStorageBytes)
            .IsRequired();

        builder.OwnsOne(x => x.Quota, quota =>
        {
            quota.Property(q => q.Bytes)
                .HasColumnName("StorageQuotaBytes")
                .IsRequired();
        });

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}