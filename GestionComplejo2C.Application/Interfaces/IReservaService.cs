using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IReservaService
    {
        ReservaResponse Crear(Guid canchaId, CrearReservaRequest request);

        IReadOnlyList<ReservaResponse> ObtenerTodas(Guid canchaId);

        ReservaResponse? ObtenerPorId(Guid canchaId, Guid id);

        bool Cancelar(Guid canchaId, Guid id);
    }
}
