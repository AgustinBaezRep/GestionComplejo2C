using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface ICourtService
    {
        Task<CourtResponse> CreateAsync(CreateCourtRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CourtResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<CourtResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<CourtResponse?> UpdatePriceAsync(Guid id, UpdatePriceRequest request, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<CourtResponse> AssignLockerRoomAsync(Guid courtId, Guid lockerRoomId, CancellationToken cancellationToken = default);

        Task<CourtResponse> RemoveLockerRoomAsync(Guid courtId, CancellationToken cancellationToken = default);

        Task<CourtResponse> AddAmenityAsync(Guid courtId, Guid amenityId, CancellationToken cancellationToken = default);

        Task<CourtResponse> RemoveAmenityAsync(Guid courtId, Guid amenityId, CancellationToken cancellationToken = default);
    }
}
