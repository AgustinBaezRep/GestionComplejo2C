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

        public IReadOnlyList<Court> GetAll() =>
            context.Courts
                .Include(c => c.Bookings).ThenInclude(b => b.Customer)
                .Include(c => c.LockerRoom)
                .Include(c => c.Amenities)
                .ToList();

        public Court? GetById(Guid id) =>
            context.Courts
                .Include(c => c.Bookings).ThenInclude(b => b.Customer)
                .Include(c => c.LockerRoom)
                .Include(c => c.Amenities)
                .FirstOrDefault(c => c.Id == id);

        public void Remove(Court court) => context.Courts.Remove(court);

        public void SaveChanges() => context.SaveChanges();
    }
}
