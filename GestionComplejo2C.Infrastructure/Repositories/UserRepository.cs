using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SportsComplexDbContext context;

        public UserRepository(SportsComplexDbContext context)
        {
            this.context = context;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
