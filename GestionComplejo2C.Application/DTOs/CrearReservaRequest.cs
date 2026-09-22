namespace GestionComplejo2C.Application.DTOs
{
    public record CrearReservaRequest(Guid ClienteId, DateTime Inicio, int Horas);
}
