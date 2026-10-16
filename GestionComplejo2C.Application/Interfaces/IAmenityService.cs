using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IAmenityService
    {
        Task<AmenityResponse> CreateAsync(CreateAmenityRequest request, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AmenityResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<AmenityResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
