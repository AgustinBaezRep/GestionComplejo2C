using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class RepositorioVestuarios : IRepositorioVestuarios
    {
        private readonly GestionComplejoDbContext context;

        public RepositorioVestuarios(GestionComplejoDbContext context)
        {
            this.context = context;
        }

        public void Agregar(Vestuario vestuario) => context.Vestuarios.Add(vestuario);

        public IReadOnlyList<Vestuario> ObtenerTodos() =>
            context.Vestuarios
                .Include(v => v.Cancha)
                .ToList();

        public Vestuario? ObtenerPorId(Guid id) =>
            context.Vestuarios
                .Include(v => v.Cancha)
                .FirstOrDefault(v => v.Id == id);

        public void Eliminar(Vestuario vestuario) => context.Vestuarios.Remove(vestuario);

        public void GuardarCambios() => context.SaveChanges();
    }
}
