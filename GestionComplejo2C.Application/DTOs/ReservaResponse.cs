using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record ReservaResponse(
        Guid Id,
        Guid CanchaId,
        Guid ClienteId,
        string? ClienteEmail,
        DateTime Inicio,
        DateTime Fin,
        int Horas,
        decimal Importe,
        bool Cancelada)
    {
        public static ReservaResponse Desde(Reserva reserva) => new(
            reserva.Id,
            reserva.CanchaId,
            reserva.ClienteId,
            reserva.Cliente?.Email,
            reserva.Inicio,
            reserva.Fin,
            reserva.Horas,
            reserva.Importe,
            reserva.Cancelada);
    }
}
