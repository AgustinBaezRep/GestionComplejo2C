using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SportsComplexDbContext context;

        public UserRepository(SportsComplexDbContext context)
        {
            this.context = context;
        }

        public User? GetByEmail(string email) =>
            context.Users.FirstOrDefault(u => u.Email == email);

        public Customer? GetCustomerById(Guid id) =>
            context.Customers.FirstOrDefault(c => c.Id == id);
    }
}
