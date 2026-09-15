using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record VestuarioResponse(
        Guid Id,
        bool Disponible,
        int Duchas,
        int Capacidad,
        Guid? CanchaId)
    {
        public static VestuarioResponse Desde(Vestuario vestuario) => new(
            vestuario.Id,
            vestuario.Disponible,
            vestuario.Duchas,
            vestuario.Capacidad,
            vestuario.Cancha?.Id);
    }
}
