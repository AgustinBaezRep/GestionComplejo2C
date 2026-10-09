using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(Guid courtId, CreateBookingRequest request);

        IReadOnlyList<BookingResponse> GetAll(Guid courtId);

        BookingResponse? GetById(Guid courtId, Guid id);

        bool Cancel(Guid courtId, Guid id);
    }
}
