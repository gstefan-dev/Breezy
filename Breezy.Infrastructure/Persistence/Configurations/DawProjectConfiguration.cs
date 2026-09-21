using Breezy.Domain.Projects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class DawProjectConfiguration : IEntityTypeConfiguration<DawProject>
{
    public void Configure(EntityTypeBuilder<DawProject> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.ProjectName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CurrentVersionNumber)
            .IsRequired();

        builder.Property(x => x.LocalPath)
            .IsRequired(false);

        builder.OwnsOne(x => x.CurrentHash, currentHash =>
            {
                currentHash.Property(h => h.Value)
                    .HasColumnName("CurrentHash")
                    .HasMaxLength(64)
                    .IsRequired();
            }
        );
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasMany(x => x.Versions)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}