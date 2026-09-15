using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record CanchaResponse(
        Guid Id,
        string Deporte,
        string TipoPiso,
        int JugadoresMax,
        decimal PrecioPorHora,
        decimal Recaudacion,
        int ReservasActivas,
        VestuarioResponse? Vestuario,
        IReadOnlyList<ServicioResponse> Servicios)
    {
        public static CanchaResponse Desde(Cancha cancha) => new(
            cancha.Id,
            cancha.Deporte,
            cancha.TipoPiso,
            cancha.JugadoresMax,
            cancha.PrecioPorHora,
            cancha.Recaudacion,
            cancha.ReservasActivas,
            cancha.Vestuario is null ? null : VestuarioResponse.Desde(cancha.Vestuario),
            cancha.Servicios.Select(ServicioResponse.Desde).ToList());
    }
}
