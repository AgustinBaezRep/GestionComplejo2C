using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface ICanchaService
    {
        CanchaResponse Crear(CrearCanchaRequest request);

        IReadOnlyList<CanchaResponse> ObtenerTodas();

        CanchaResponse? ObtenerPorId(Guid id);

        CanchaResponse? ActualizarPrecio(Guid id, ActualizarPrecioRequest request);

        bool Eliminar(Guid id);

        CanchaResponse AsignarVestuario(Guid canchaId, Guid vestuarioId);

        CanchaResponse QuitarVestuario(Guid canchaId);

        CanchaResponse AgregarServicio(Guid canchaId, Guid servicioId);

        CanchaResponse QuitarServicio(Guid canchaId, Guid servicioId);
    }
}
