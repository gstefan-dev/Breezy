using Breezy.Domain.Users.Entities;

namespace Breezy.Application.Interfaces;

public interface IAuthService
{
    Task<string> GenerateTokenAsync(User user);
    Task<Guid?> ValidateTokenAsync(string token);
    Task<string> HashPasswordAsync(string password);
}