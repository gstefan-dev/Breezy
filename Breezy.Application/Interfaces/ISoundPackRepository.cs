using Breezy.Domain.SoundPacks.Entities;

namespace Breezy.Application.Interfaces;

public interface ISoundPackRepository
{
    Task AddAsync(SoundPack soundPack);
    Task<SoundPack?> GetByIdAsync(Guid id);
    Task<IEnumerable<SoundPack>> GetByUserIdAsync(Guid id);
    Task UpdateAsync(SoundPack soundPack);
}