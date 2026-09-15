using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IServicioService
    {
        ServicioResponse Crear(CrearServicioRequest request);

        IReadOnlyList<ServicioResponse> ObtenerTodos();

        ServicioResponse? ObtenerPorId(Guid id);

        bool Eliminar(Guid id);
    }
}
