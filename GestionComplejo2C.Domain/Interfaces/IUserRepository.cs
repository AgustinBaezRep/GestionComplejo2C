using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<Customer?> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
