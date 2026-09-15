using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IRepositorioCanchas repositorioCanchas;

        public ReservaService(IRepositorioCanchas repositorioCanchas)
        {
            this.repositorioCanchas = repositorioCanchas;
        }

        public ReservaResponse Crear(Guid canchaId, CrearReservaRequest request)
        {
            var cancha = ObtenerCancha(canchaId);

            var reserva = cancha.Reservar(request.Cliente, request.Inicio, request.Horas);

            repositorioCanchas.AgregarReserva(reserva);
            repositorioCanchas.GuardarCambios();

            return ReservaResponse.Desde(reserva);
        }

        public IReadOnlyList<ReservaResponse> ObtenerTodas(Guid canchaId) =>
            ObtenerCancha(canchaId).VerHistorial().Select(ReservaResponse.Desde).ToList();

        public ReservaResponse? ObtenerPorId(Guid canchaId, Guid id)
        {
            var reserva = ObtenerCancha(canchaId).ObtenerReserva(id);

            return reserva == null ? null : ReservaResponse.Desde(reserva);
        }

        public bool Cancelar(Guid canchaId, Guid id)
        {
            var cancha = ObtenerCancha(canchaId);

            if (cancha.ObtenerReserva(id) == null)
            {
                return false;
            }

            cancha.Cancelar(id);

            repositorioCanchas.GuardarCambios();

            return true;
        }

        private Cancha ObtenerCancha(Guid canchaId) =>
            repositorioCanchas.ObtenerPorId(canchaId)
                ?? throw new KeyNotFoundException($"There is no element that match with the id {canchaId}");
    }
}
