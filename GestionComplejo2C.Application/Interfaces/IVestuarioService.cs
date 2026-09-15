using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IVestuarioService
    {
        VestuarioResponse Crear(CrearVestuarioRequest request);

        IReadOnlyList<VestuarioResponse> ObtenerTodos();

        VestuarioResponse? ObtenerPorId(Guid id);

        bool Eliminar(Guid id);
    }
}
