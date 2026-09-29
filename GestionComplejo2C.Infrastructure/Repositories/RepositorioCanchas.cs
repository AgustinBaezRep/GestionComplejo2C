using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        public void AgregarReserva(Reserva reserva) => context.Reservas.Add(reserva);

        public IReadOnlyList<Cancha> ObtenerTodas() =>
            context.Canchas
                .Include(c => c.Reservas).ThenInclude(r => r.Cliente)
                .Include(c => c.Vestuario)
                .Include(c => c.Servicios)
                .ToList();

        public Cancha? ObtenerPorId(Guid id) =>
            context.Canchas
                .Include(c => c.Reservas).ThenInclude(r => r.Cliente)
                .Include(c => c.Vestuario)
                .Include(c => c.Servicios)
                .FirstOrDefault(c => c.Id == id);

        public void Eliminar(Cancha cancha) => context.Canchas.Remove(cancha);

        public void GuardarCambios() => context.SaveChanges();
    }
}
