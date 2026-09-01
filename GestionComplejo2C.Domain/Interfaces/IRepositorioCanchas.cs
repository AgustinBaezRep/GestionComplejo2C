using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IRepositorioCanchas
    {
        void Agregar(Cancha cancha);

        IReadOnlyList<Cancha> ObtenerTodas();

        Cancha? ObtenerPorId(int id);

        bool Eliminar(Cancha cancha);
    }
}
