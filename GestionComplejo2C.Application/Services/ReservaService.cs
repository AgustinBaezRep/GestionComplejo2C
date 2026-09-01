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

        public Reserva Crear(int canchaId, CrearReservaRequest request)
        {
            var cancha = ObtenerCancha(canchaId);

            return cancha.Reservar(request.Cliente, request.Inicio, request.Horas);
        }

        public IReadOnlyList<Reserva> ObtenerTodas(int canchaId) => ObtenerCancha(canchaId).VerHistorial();

        public Reserva? ObtenerPorId(int canchaId, Guid id) => ObtenerCancha(canchaId).ObtenerReserva(id);

        public bool Cancelar(int canchaId, Guid id)
        {
            var cancha = ObtenerCancha(canchaId);

            if (cancha.ObtenerReserva(id) == null)
            {
                return false;
            }

            cancha.Cancelar(id);

            return true;
        }

        private Cancha ObtenerCancha(int canchaId) =>
            repositorioCanchas.ObtenerPorId(canchaId)
                ?? throw new KeyNotFoundException($"There is no element that match with the id {canchaId}");
    }
}
