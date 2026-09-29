using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IAmenityRepository
    {
        void Add(Amenity amenity);

        IReadOnlyList<Amenity> GetAll();

        Amenity? GetById(Guid id);

        void Remove(Amenity amenity);

        void SaveChanges();
    }
}
