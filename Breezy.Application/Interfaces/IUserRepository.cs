using Breezy.Domain.Users.Entities;
using Breezy.Domain.Users.ValueObjects;

namespace Breezy.Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetUserByEmailAsync(Email email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task UpdateAsync(User user);
}