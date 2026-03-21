using Eprocurement.Domain.Entities;
using Eprocurement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eprocurement.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProcurementDbContext _context;

        public UserRepository(ProcurementDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
            => _context.Users.AddAsync(user, cancellationToken).AsTask();

        public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => _context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
            => _context.Users.AnyAsync(x => x.Email == email, cancellationToken);
    }
}