using Breezy.Domain.Projects.Entities;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.SoundPacks.Entities;
using Breezy.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Breezy.Infrastructure.Persistence;

public class BreezyDbContext : DbContext
{
    public BreezyDbContext(DbContextOptions<BreezyDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<DawProject> Projects { get; set; }
    public DbSet<CollaborationSession> Sessions { get; set; }
    public DbSet<SoundPack> SoundPacks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BreezyDbContext).Assembly);
    }
}



