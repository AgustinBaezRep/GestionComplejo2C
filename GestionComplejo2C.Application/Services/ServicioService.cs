using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IRepositorioServicios repositorioServicios;

        public ServicioService(IRepositorioServicios repositorioServicios)
        {
            this.repositorioServicios = repositorioServicios;
        }

        public ServicioResponse Crear(CrearServicioRequest request)
        {
            var servicio = new Servicio(request.Nombre, request.Descripcion, request.Costo);

            repositorioServicios.Agregar(servicio);
            repositorioServicios.GuardarCambios();

            return ServicioResponse.Desde(servicio);
        }

        public IReadOnlyList<ServicioResponse> ObtenerTodos() =>
            repositorioServicios.ObtenerTodos().Select(ServicioResponse.Desde).ToList();

        public ServicioResponse? ObtenerPorId(Guid id)
        {
            var servicio = repositorioServicios.ObtenerPorId(id);

            return servicio == null ? null : ServicioResponse.Desde(servicio);
        }

        public bool Eliminar(Guid id)
        {
            var servicio = repositorioServicios.ObtenerPorId(id);

            if (servicio == null)
            {
                return false;
            }

            repositorioServicios.Eliminar(servicio);
            repositorioServicios.GuardarCambios();

            return true;
        }
    }
}
