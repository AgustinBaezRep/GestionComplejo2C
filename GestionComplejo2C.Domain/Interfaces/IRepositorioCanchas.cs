using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IRepositorioCanchas
    {
        void Agregar(Cancha cancha);

        IReadOnlyList<Cancha> ObtenerTodas();

        Cancha? ObtenerPorId(Guid id);

        void Eliminar(Cancha cancha);

        void GuardarCambios();
    }
}
