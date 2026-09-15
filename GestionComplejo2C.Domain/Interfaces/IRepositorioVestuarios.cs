using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IRepositorioVestuarios
    {
        void Agregar(Vestuario vestuario);

        IReadOnlyList<Vestuario> ObtenerTodos();

        Vestuario? ObtenerPorId(Guid id);

        void Eliminar(Vestuario vestuario);

        void GuardarCambios();
    }
}
