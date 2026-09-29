using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record AmenityResponse(
        Guid Id,
        string Name,
        string Description,
        decimal Cost)
    {
        public static AmenityResponse From(Amenity amenity) => new(
            amenity.Id,
            amenity.Name,
            amenity.Description,
            amenity.Cost);
    }
}
