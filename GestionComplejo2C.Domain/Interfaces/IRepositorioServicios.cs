using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IRepositorioServicios
    {
        void Agregar(Servicio servicio);

        IReadOnlyList<Servicio> ObtenerTodos();

        Servicio? ObtenerPorId(Guid id);

        void Eliminar(Servicio servicio);

        void GuardarCambios();
    }
}
