using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface ICanchaService
    {
        Cancha Crear(CrearCanchaRequest request);

        IReadOnlyList<Cancha> ObtenerTodas();

        Cancha? ObtenerPorId(int id);

        Cancha? ActualizarPrecio(int id, ActualizarPrecioRequest request);

        bool Eliminar(int id);
    }
}
