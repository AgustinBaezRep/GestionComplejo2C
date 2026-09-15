using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class RepositorioServicios : IRepositorioServicios
    {
        private readonly GestionComplejoDbContext context;

        public RepositorioServicios(GestionComplejoDbContext context)
        {
            this.context = context;
        }

        public void Agregar(Servicio servicio) => context.Servicios.Add(servicio);

        public IReadOnlyList<Servicio> ObtenerTodos() => context.Servicios.ToList();

        public Servicio? ObtenerPorId(Guid id) => context.Servicios.FirstOrDefault(s => s.Id == id);

        public void Eliminar(Servicio servicio) => context.Servicios.Remove(servicio);

        public void GuardarCambios() => context.SaveChanges();
    }
}
