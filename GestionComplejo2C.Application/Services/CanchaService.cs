using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class CanchaService : ICanchaService
    {
        private readonly IRepositorioCanchas repositorioCanchas;

        public CanchaService(IRepositorioCanchas repositorioCanchas)
        {
            this.repositorioCanchas = repositorioCanchas;
        }

        public Cancha Crear(CrearCanchaRequest request)
        {
            var cancha = new Cancha(request.Deporte, request.TipoPiso, request.JugadoresMax, request.PrecioPorHora);

            repositorioCanchas.Agregar(cancha);
            repositorioCanchas.GuardarCambios();

            return cancha;
        }

        public IReadOnlyList<Cancha> ObtenerTodas() => repositorioCanchas.ObtenerTodas();

        public Cancha? ObtenerPorId(Guid id) => repositorioCanchas.ObtenerPorId(id);

        public Cancha? ActualizarPrecio(Guid id, ActualizarPrecioRequest request)
        {
            var cancha = ObtenerPorId(id);

            if (cancha == null)
            {
                return null;
            }

            cancha.ActualizarPrecio(request.PrecioPorHora);

            repositorioCanchas.GuardarCambios();

            return cancha;
        }

        public bool Eliminar(Guid id)
        {
            var cancha = ObtenerPorId(id);

            if (cancha == null)
            {
                return false;
            }

            if (cancha.ReservasActivas > 0)
            {
                throw new InvalidOperationException($"The court {id} has active bookings");
            }

            repositorioCanchas.Eliminar(cancha);
            repositorioCanchas.GuardarCambios();

            return true;
        }
    }
}
