using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IReservaService
    {
        Reserva Crear(Guid canchaId, CrearReservaRequest request);

        IReadOnlyList<Reserva> ObtenerTodas(Guid canchaId);

        Reserva? ObtenerPorId(Guid canchaId, Guid id);

        bool Cancelar(Guid canchaId, Guid id);
    }
}
