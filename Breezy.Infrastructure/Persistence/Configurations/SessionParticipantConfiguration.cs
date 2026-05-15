using Breezy.Domain.Sessions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Breezy.Infrastructure.Persistence.Configurations;

public class SessionParticipantConfiguration : IEntityTypeConfiguration<SessionParticipant>
{
    public void Configure(EntityTypeBuilder<SessionParticipant> builder)
    {
        builder.ToTable("SessionParticipants");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.SessionId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.JoinedAt).IsRequired();
    }
}