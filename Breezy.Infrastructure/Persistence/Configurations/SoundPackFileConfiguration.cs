using Breezy.Domain.SoundPacks.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class SoundPackFileConfiguration : IEntityTypeConfiguration<SoundPackFile>
{
    public void Configure(EntityTypeBuilder<SoundPackFile> builder)
    {
        builder.ToTable("SoundPackFiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SoundPackId).IsRequired();
        builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
        builder.Property(x => x.BlobUrl).IsRequired();
        builder.Property(x => x.DurationInSeconds).IsRequired();
        builder.Property(x => x.Bpm).IsRequired();
        builder.Property(x => x.Key).HasMaxLength(10);
        builder.Property(x => x.FileSize).IsRequired();
        
        builder.Property(x => x.Category)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}