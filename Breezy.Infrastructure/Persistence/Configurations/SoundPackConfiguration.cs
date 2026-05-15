using Breezy.Domain.SoundPacks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class SoundPackConfiguration : IEntityTypeConfiguration<SoundPack>
{
    public void Configure(EntityTypeBuilder<SoundPack> builder)
    {
        builder.ToTable("SoundPacks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerUserId)
            .IsRequired();

        builder.Property(x => x.PackName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PackCategory)
            .HasMaxLength(50);

        builder.Property(x => x.OwnerName)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.LastExport, export =>
        {
            export.Property(e => e.Format)
                .HasMaxLength(10)
                .HasConversion<string>();

            export.Property(e => e.BlobUrl);

            export.Property(x => x.ExportedAt);

            export.Property(x => x.SoundPackId);

        });
        builder.Navigation(x => x.LastExport).IsRequired(false);
        
        builder.Property(x => x.Mood)
            .HasMaxLength(50);

        builder.Property(x => x.Genre)
            .HasMaxLength(50);

        builder.HasMany(x => x.Files)
            .WithOne()
            .HasForeignKey(x => x.SoundPackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);
    }
}