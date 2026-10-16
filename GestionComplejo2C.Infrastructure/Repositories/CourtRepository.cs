using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class CourtRepository : ICourtRepository
    {
        private readonly SportsComplexDbContext context;

        public CourtRepository(SportsComplexDbContext context)
        {
            this.context = context;
        }

        public void Add(Court court) => context.Courts.Add(court);

        public void AddBooking(Booking booking) => context.Bookings.Add(booking);

        public async Task<IReadOnlyList<Court>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await context.Courts
                .Include(c => c.Bookings).ThenInclude(b => b.Customer)
                .Include(c => c.LockerRoom)
                .Include(c => c.Amenities)
                .ToListAsync(cancellationToken);

        public Task<Court?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.Courts
                .Include(c => c.Bookings).ThenInclude(b => b.Customer)
                .Include(c => c.LockerRoom)
                .Include(c => c.Amenities)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public void Remove(Court court) => context.Courts.Remove(court);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);
    }
}
