using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;
using GestionComplejo2C.Infrastructure.Persistence;

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

        public IReadOnlyList<Amenity> GetAll() => context.Amenities.ToList();

        public Amenity? GetById(Guid id) => context.Amenities.FirstOrDefault(a => a.Id == id);

        public void Remove(Amenity amenity) => context.Amenities.Remove(amenity);

        public void SaveChanges() => context.SaveChanges();
    }
}
