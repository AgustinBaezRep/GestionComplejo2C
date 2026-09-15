using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record ServicioResponse(
        Guid Id,
        string Nombre,
        string Descripcion,
        decimal Costo)
    {
        public static ServicioResponse Desde(Servicio servicio) => new(
            servicio.Id,
            servicio.Nombre,
            servicio.Descripcion,
            servicio.Costo);
    }
}
