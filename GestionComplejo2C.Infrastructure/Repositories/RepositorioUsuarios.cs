using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly GestionComplejoDbContext context;

        public RepositorioUsuarios(GestionComplejoDbContext context)
        {
            this.context = context;
        }

        public Usuario? ObtenerPorEmail(string email) =>
            context.Usuarios.FirstOrDefault(u => u.Email == email);

        public Cliente? ObtenerClientePorId(Guid id) =>
            context.Clientes.FirstOrDefault(c => c.Id == id);
    }
}
