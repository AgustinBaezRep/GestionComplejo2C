using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateAsync(Guid courtId, CreateBookingRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<BookingResponse>> GetAllAsync(Guid courtId, CancellationToken cancellationToken = default);

        Task<BookingResponse?> GetByIdAsync(Guid courtId, Guid id, CancellationToken cancellationToken = default);

        Task<bool> CancelAsync(Guid courtId, Guid id, CancellationToken cancellationToken = default);
    }
}
