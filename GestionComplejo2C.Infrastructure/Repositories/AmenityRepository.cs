using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionComplejo2C.Infrastructure.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly SportsComplexDbContext context;

        public AmenityRepository(SportsComplexDbContext context)
        {
            this.context = context;
        }

        public void Add(Amenity amenity) => context.Amenities.Add(amenity);

        public async Task<IReadOnlyList<Amenity>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await context.Amenities.ToListAsync(cancellationToken);

        public Task<Amenity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.Amenities.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        public void Remove(Amenity amenity) => context.Amenities.Remove(amenity);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);
    }
}
