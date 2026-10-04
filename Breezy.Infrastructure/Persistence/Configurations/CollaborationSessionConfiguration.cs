using Breezy.Domain.Sessions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class CollaborationSessionConfiguration : IEntityTypeConfiguration<CollaborationSession>
{
    public void Configure(EntityTypeBuilder<CollaborationSession> builder)
    {
        builder.ToTable("Sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.OwnerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.OwnsOne(x => x.Code, code =>
        {
            code.Property(c => c.Value)
                .IsRequired()
                .HasColumnName("Code")
                .HasMaxLength(10);
        });

        builder.Property(x => x.MaxParticipants)
            .IsRequired();

        builder.Property(x => x.IsClosedByOwner)
            .IsRequired();

        builder.Property(x => x.ExpiresAt);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
        
        builder.Property(x => x.Visibility)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.HasMany(x => x.Participants)  
            .WithOne()                         
            .HasForeignKey(x => x.SessionId)   
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Submissions)
            .WithOne()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}