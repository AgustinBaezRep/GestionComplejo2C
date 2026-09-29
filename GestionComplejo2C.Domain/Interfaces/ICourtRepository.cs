using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Domain.Interfaces
{
    public interface ICourtRepository
    {
        void Add(Court court);

        void AddBooking(Booking booking);

        IReadOnlyList<Court> GetAll();

        Court? GetById(Guid id);

        void Remove(Court court);

        void SaveChanges();
    }
}
