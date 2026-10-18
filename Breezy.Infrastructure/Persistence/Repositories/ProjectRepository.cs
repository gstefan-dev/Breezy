using Breezy.Application.Interfaces;
using Breezy.Domain.Projects.Entities;
using Microsoft.EntityFrameworkCore;

namespace Breezy.Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly BreezyDbContext _context;

    public ProjectRepository(BreezyDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DawProject project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task<DawProject?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<DawProject>> GetByUserIdAsync(Guid id)
    {
        return await _context.Projects.Where(p => p.OwnerUserId == id).ToListAsync();
    }

    public async Task UpdateAsync(DawProject project)
    {
        await _context.SaveChangesAsync();
    }
    public async Task AddVersionAsync(ProjectVersion version)
    {
        await _context.Set<ProjectVersion>().AddAsync(version);
        await _context.SaveChangesAsync();
    }
}