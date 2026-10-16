using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository amenityRepository;

        public AmenityService(IAmenityRepository amenityRepository)
        {
            this.amenityRepository = amenityRepository;
        }

        public async Task<AmenityResponse> CreateAsync(CreateAmenityRequest request, CancellationToken cancellationToken = default)
        {
            var amenity = new Amenity(request.Name, request.Description, request.Cost);

            amenityRepository.Add(amenity);
            await amenityRepository.SaveChangesAsync(cancellationToken);

            return AmenityResponse.From(amenity);
        }

        public async Task<IReadOnlyList<AmenityResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var amenities = await amenityRepository.GetAllAsync(cancellationToken);

            return amenities.Select(AmenityResponse.From).ToList();
        }

        public async Task<AmenityResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var amenity = await amenityRepository.GetByIdAsync(id, cancellationToken);

            return amenity == null ? null : AmenityResponse.From(amenity);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var amenity = await amenityRepository.GetByIdAsync(id, cancellationToken);

            if (amenity == null)
            {
                return false;
            }

            amenityRepository.Remove(amenity);
            await amenityRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
