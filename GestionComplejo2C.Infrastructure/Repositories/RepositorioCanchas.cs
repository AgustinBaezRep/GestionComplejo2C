using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class RepositorioCanchas : IRepositorioCanchas
    {
        private readonly GestionComplejoDbContext context;

        public RepositorioCanchas(GestionComplejoDbContext context)
        {
            this.context = context;
        }

        public void Agregar(Cancha cancha) => context.Canchas.Add(cancha);

        public IReadOnlyList<Cancha> ObtenerTodas() => context.Canchas.ToList();

        public Cancha? ObtenerPorId(Guid id) => context.Canchas.FirstOrDefault(c => c.Id == id);

        public void Eliminar(Cancha cancha) => context.Canchas.Remove(cancha);

        public void GuardarCambios() => context.SaveChanges();
    }
}
