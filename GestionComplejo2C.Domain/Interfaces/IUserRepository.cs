using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);

        Customer? GetCustomerById(Guid id);
    }
}
