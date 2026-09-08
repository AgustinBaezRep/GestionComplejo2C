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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cancha>()
                .HasMany<Reserva>("reservas")
                .WithOne()
                .HasForeignKey(r => r.CanchaId);

            modelBuilder.Entity<Cancha>()
                .Navigation("reservas")
                .AutoInclude();
        }
    }
}
