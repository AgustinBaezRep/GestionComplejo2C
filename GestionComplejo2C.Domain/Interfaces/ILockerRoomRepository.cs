using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface ILockerRoomRepository
    {
        void Add(LockerRoom lockerRoom);

        Task<IReadOnlyList<LockerRoom>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<LockerRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        void Remove(LockerRoom lockerRoom);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
