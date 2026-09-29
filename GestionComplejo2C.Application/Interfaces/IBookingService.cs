using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IBookingService
    {
        BookingResponse Create(Guid courtId, CreateBookingRequest request);

        IReadOnlyList<BookingResponse> GetAll(Guid courtId);

        BookingResponse? GetById(Guid courtId, Guid id);

        bool Cancel(Guid courtId, Guid id);
    }
}
