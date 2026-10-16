using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface ICourtRepository
    {
        void Add(Court court);

        void AddBooking(Booking booking);

        Task<IReadOnlyList<Court>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Court?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        void Remove(Court court);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
