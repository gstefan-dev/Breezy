using Breezy.Domain.Projects.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectVersionConfiguration : IEntityTypeConfiguration<ProjectVersion>
{
    public void Configure(EntityTypeBuilder<ProjectVersion> builder)
    {
        builder.ToTable("ProjectVersions");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.ProjectId).IsRequired();
        builder.Property(x => x.VersionNumber).IsRequired();
        builder.Property(x => x.BlobUrl).IsRequired();
        builder.Property(x => x.UploadedAt).IsRequired();
        
        builder.OwnsOne(x => x.Hash, hash =>
        {
            hash.Property(h => h.Value)
                .HasColumnName("Hash")
                .HasMaxLength(64)
                .IsRequired();
        });

        builder.OwnsOne(x => x.Changelog, changelog =>
        {
            changelog.Property(c => c.Changes)
                .HasColumnName("Changelog");
        });
    }
}