using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface ILockerRoomService
    {
        Task<LockerRoomResponse> CreateAsync(CreateLockerRoomRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<LockerRoomResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<LockerRoomResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
