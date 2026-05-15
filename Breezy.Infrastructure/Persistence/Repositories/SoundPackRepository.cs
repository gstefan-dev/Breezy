using Breezy.Application.Interfaces;
using Breezy.Domain.SoundPacks.Entities;
using Microsoft.EntityFrameworkCore;

namespace Breezy.Infrastructure.Persistence.Repositories;

public class SoundPackRepository : ISoundPackRepository
{
    private readonly BreezyDbContext _context;

    public SoundPackRepository(BreezyDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SoundPack soundPack)
    {
        await _context.SoundPacks.AddAsync(soundPack);
        await _context.SaveChangesAsync();
    }

    public async Task<SoundPack?> GetByIdAsync(Guid id)
    {
        return await _context.SoundPacks.FindAsync(id);
    }

    public async Task<IEnumerable<SoundPack>> GetByUserIdAsync(Guid id)
    {
        return await _context.SoundPacks.Where(s => s.OwnerUserId == id).ToListAsync();
    }

    public async Task UpdateAsync(SoundPack soundPack)
    {
        _context.SoundPacks.Update(soundPack);
        await _context.SaveChangesAsync();
    }
}