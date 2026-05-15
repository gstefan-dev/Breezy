    using Breezy.Application.Interfaces;
    using Breezy.Domain.Users.Entities;
    using Breezy.Domain.Users.ValueObjects;
    using Microsoft.EntityFrameworkCore;

    namespace Breezy.Infrastructure.Persistence.Repositories;

    public class UserRepository : IUserRepository
    {
        private readonly BreezyDbContext _context;

        public UserRepository(BreezyDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(Email email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }