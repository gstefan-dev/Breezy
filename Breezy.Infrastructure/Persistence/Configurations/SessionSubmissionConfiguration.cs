using Breezy.Domain.Sessions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class SessionSubmissionConfiguration : IEntityTypeConfiguration<SessionSubmission>
{
    public void Configure(EntityTypeBuilder<SessionSubmission> builder)
    {
        builder.ToTable("SessionSubmissions");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.SessionId).IsRequired();
        builder.Property(x => x.SubmittedByUserId).IsRequired();
        builder.Property(x => x.SubmitterName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.BlobUrl).IsRequired();
        
        builder.OwnsOne(x => x.Metadata, metadata =>
        {
            metadata.Property(m => m.FileName).HasColumnName("FileName").IsRequired();
            metadata.Property(m => m.DurationSeconds).HasColumnName("DurationSeconds").IsRequired();
            metadata.Property(m => m.BPM).HasColumnName("BPM").IsRequired();
            metadata.Property(m => m.Genre).HasColumnName("Genre");
            metadata.Property(m => m.Key).HasColumnName("Key").IsRequired();
            metadata.Property(m => m.SubmittedAt).HasColumnName("SubmittedAt").IsRequired();
        });
    }
}