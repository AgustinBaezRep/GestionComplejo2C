using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class VestuarioService : IVestuarioService
    {
        private readonly IRepositorioVestuarios repositorioVestuarios;

        public VestuarioService(IRepositorioVestuarios repositorioVestuarios)
        {
            this.repositorioVestuarios = repositorioVestuarios;
        }

        public VestuarioResponse Crear(CrearVestuarioRequest request)
        {
            var vestuario = new Vestuario(request.Disponible, request.Duchas, request.Capacidad);

            repositorioVestuarios.Agregar(vestuario);
            repositorioVestuarios.GuardarCambios();

            return VestuarioResponse.Desde(vestuario);
        }

        public IReadOnlyList<VestuarioResponse> ObtenerTodos() =>
            repositorioVestuarios.ObtenerTodos().Select(VestuarioResponse.Desde).ToList();

        public VestuarioResponse? ObtenerPorId(Guid id)
        {
            var vestuario = repositorioVestuarios.ObtenerPorId(id);

            return vestuario == null ? null : VestuarioResponse.Desde(vestuario);
        }

        public bool Eliminar(Guid id)
        {
            var vestuario = repositorioVestuarios.ObtenerPorId(id);

            if (vestuario == null)
            {
                return false;
            }

            repositorioVestuarios.Eliminar(vestuario);
            repositorioVestuarios.GuardarCambios();

            return true;
        }
    }
}
