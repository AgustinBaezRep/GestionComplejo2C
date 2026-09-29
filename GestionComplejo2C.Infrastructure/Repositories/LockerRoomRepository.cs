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

        public IReadOnlyList<LockerRoom> GetAll() =>
            context.LockerRooms
                .Include(l => l.Court)
                .ToList();

        public LockerRoom? GetById(Guid id) =>
            context.LockerRooms
                .Include(l => l.Court)
                .FirstOrDefault(l => l.Id == id);

        public void Remove(LockerRoom lockerRoom) => context.LockerRooms.Remove(lockerRoom);

        public void SaveChanges() => context.SaveChanges();
    }
}
