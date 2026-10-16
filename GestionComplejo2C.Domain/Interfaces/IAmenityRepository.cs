using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IAmenityRepository
    {
        void Add(Amenity amenity);

        Task<IReadOnlyList<Amenity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        void Remove(Amenity amenity);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
