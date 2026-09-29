using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface ILockerRoomRepository
    {
        void Add(LockerRoom lockerRoom);

        IReadOnlyList<LockerRoom> GetAll();

        LockerRoom? GetById(Guid id);

        void Remove(LockerRoom lockerRoom);

        void SaveChanges();
    }
}
