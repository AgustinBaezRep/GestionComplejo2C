using GestionComplejo2C.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Persistence
{
    public class GestionComplejoDbContext : DbContext
    {
        public GestionComplejoDbContext(DbContextOptions<GestionComplejoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cancha> Canchas => Set<Cancha>();

        public DbSet<Reserva> Reservas => Set<Reserva>();

        public DbSet<Vestuario> Vestuarios => Set<Vestuario>();

        public DbSet<Servicio> Servicios => Set<Servicio>();

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Administrador> Administradores => Set<Administrador>();

        public DbSet<Cliente> Clientes => Set<Cliente>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().UseTpcMappingStrategy();
        }
    }
}
