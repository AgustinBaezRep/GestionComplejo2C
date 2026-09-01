using GestionComplejo2C.Presentation.Data;
using GestionComplejo2C.Presentation.DTOs;
using GestionComplejo2C.Presentation.Models;

namespace GestionComplejo2C.Presentation.Services
{
    public class CanchaService : ICanchaService
    {
        public Cancha Crear(CrearCanchaRequest request)
        {
            var cancha = new Cancha(request.Deporte, request.TipoPiso, request.JugadoresMax, request.PrecioPorHora);

            RepositorioCanchas.Agregar(cancha);

            return cancha;
        }

        public IReadOnlyList<Cancha> ObtenerTodas() => RepositorioCanchas.ObtenerTodas();

        public Cancha? ObtenerPorId(int id) => RepositorioCanchas.ObtenerPorId(id);

        public Cancha? ActualizarPrecio(int id, ActualizarPrecioRequest request)
        {
            var cancha = ObtenerPorId(id);

            if (cancha == null)
            {
                return null;
            }

            cancha.ActualizarPrecio(request.PrecioPorHora);

            return cancha;
        }

        public bool Eliminar(int id)
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

            if (!RepositorioCanchas.Eliminar(cancha))
            {
                throw new InvalidOperationException($"Problem to delete the item {id}");
            }

            return true;
        }
    }
}
