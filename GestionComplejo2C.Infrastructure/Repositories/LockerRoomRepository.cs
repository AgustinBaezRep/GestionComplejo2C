using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class LockerRoomRepository : ILockerRoomRepository
    {
        private readonly SportsComplexDbContext context;

        public LockerRoomRepository(SportsComplexDbContext context)
        {
            this.context = context;
        }

        public void Add(LockerRoom lockerRoom) => context.LockerRooms.Add(lockerRoom);

        public async Task<IReadOnlyList<LockerRoom>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await context.LockerRooms
                .Include(l => l.Court)
                .ToListAsync(cancellationToken);

        public Task<LockerRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.LockerRooms
                .Include(l => l.Court)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        public void Remove(LockerRoom lockerRoom) => context.LockerRooms.Remove(lockerRoom);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);
    }
}
