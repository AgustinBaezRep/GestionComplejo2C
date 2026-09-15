using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class CanchaService : ICanchaService
    {
        private readonly IRepositorioCanchas repositorioCanchas;
        private readonly IRepositorioVestuarios repositorioVestuarios;
        private readonly IRepositorioServicios repositorioServicios;

        public CanchaService(
            IRepositorioCanchas repositorioCanchas,
            IRepositorioVestuarios repositorioVestuarios,
            IRepositorioServicios repositorioServicios)
        {
            this.repositorioCanchas = repositorioCanchas;
            this.repositorioVestuarios = repositorioVestuarios;
            this.repositorioServicios = repositorioServicios;
        }

        public CanchaResponse Crear(CrearCanchaRequest request)
        {
            var cancha = new Cancha(request.Deporte, request.TipoPiso, request.JugadoresMax, request.PrecioPorHora);

            repositorioCanchas.Agregar(cancha);
            repositorioCanchas.GuardarCambios();

            return CanchaResponse.Desde(cancha);
        }

        public IReadOnlyList<CanchaResponse> ObtenerTodas() =>
            repositorioCanchas.ObtenerTodas().Select(CanchaResponse.Desde).ToList();

        public CanchaResponse? ObtenerPorId(Guid id)
        {
            var cancha = repositorioCanchas.ObtenerPorId(id);

            return cancha == null ? null : CanchaResponse.Desde(cancha);
        }

        public CanchaResponse? ActualizarPrecio(Guid id, ActualizarPrecioRequest request)
        {
            var cancha = repositorioCanchas.ObtenerPorId(id);

            if (cancha == null)
            {
                return null;
            }

            cancha.ActualizarPrecio(request.PrecioPorHora);

            repositorioCanchas.GuardarCambios();

            return CanchaResponse.Desde(cancha);
        }

        public bool Eliminar(Guid id)
        {
            var cancha = repositorioCanchas.ObtenerPorId(id);

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

        public CanchaResponse AsignarVestuario(Guid canchaId, Guid vestuarioId)
        {
            var cancha = ObtenerCancha(canchaId);

            var vestuario = repositorioVestuarios.ObtenerPorId(vestuarioId)
                ?? throw new KeyNotFoundException($"There is no locker room that match with the id {vestuarioId}");

            if (vestuario.Cancha != null && vestuario.Cancha.Id != canchaId)
            {
                throw new InvalidOperationException($"The locker room {vestuarioId} is already assigned to another court");
            }

            cancha.AsignarVestuario(vestuario);

            repositorioCanchas.GuardarCambios();

            return CanchaResponse.Desde(cancha);
        }

        public CanchaResponse QuitarVestuario(Guid canchaId)
        {
            var cancha = ObtenerCancha(canchaId);

            cancha.QuitarVestuario();

            repositorioCanchas.GuardarCambios();

            return CanchaResponse.Desde(cancha);
        }

        public CanchaResponse AgregarServicio(Guid canchaId, Guid servicioId)
        {
            var cancha = ObtenerCancha(canchaId);

            var servicio = repositorioServicios.ObtenerPorId(servicioId)
                ?? throw new KeyNotFoundException($"There is no service that match with the id {servicioId}");

            cancha.AgregarServicio(servicio);

            repositorioCanchas.GuardarCambios();

            return CanchaResponse.Desde(cancha);
        }

        public CanchaResponse QuitarServicio(Guid canchaId, Guid servicioId)
        {
            var cancha = ObtenerCancha(canchaId);

            cancha.QuitarServicio(servicioId);

            repositorioCanchas.GuardarCambios();

            return CanchaResponse.Desde(cancha);
        }

        private Cancha ObtenerCancha(Guid canchaId) =>
            repositorioCanchas.ObtenerPorId(canchaId)
                ?? throw new KeyNotFoundException($"There is no element that match with the id {canchaId}");
    }
}
