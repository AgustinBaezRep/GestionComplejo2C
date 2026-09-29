using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IAmenityService
    {
        AmenityResponse Create(CreateAmenityRequest request);

        IReadOnlyList<AmenityResponse> GetAll();

        AmenityResponse? GetById(Guid id);

        bool Delete(Guid id);
    }
}
