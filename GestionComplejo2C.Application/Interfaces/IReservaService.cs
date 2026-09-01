using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IReservaService
    {
        Reserva Crear(int canchaId, CrearReservaRequest request);

        IReadOnlyList<Reserva> ObtenerTodas(int canchaId);

        Reserva? ObtenerPorId(int canchaId, Guid id);

        bool Cancelar(int canchaId, Guid id);
    }
}
