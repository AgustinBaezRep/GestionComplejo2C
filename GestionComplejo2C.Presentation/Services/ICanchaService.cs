using GestionComplejo2C.Presentation.DTOs;
using GestionComplejo2C.Presentation.Models;

namespace GestionComplejo2C.Presentation.Services
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
