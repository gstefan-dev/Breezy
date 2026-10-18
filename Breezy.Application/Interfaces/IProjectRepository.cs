using Breezy.Domain.Projects.Entities;

namespace Breezy.Application.Interfaces;

public interface IProjectRepository
{
    Task AddAsync(DawProject project);
    Task<DawProject?> GetByIdAsync(Guid id);
    Task<IEnumerable<DawProject>> GetByUserIdAsync(Guid id);
    Task UpdateAsync(DawProject project);
    Task AddVersionAsync(ProjectVersion version);
}