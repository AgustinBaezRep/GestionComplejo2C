using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IRepositorioUsuarios
    {
        Usuario? ObtenerPorEmail(string email);

        Cliente? ObtenerClientePorId(Guid id);
    }
}
