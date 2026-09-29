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

        public AmenityResponse Create(CreateAmenityRequest request)
        {
            var amenity = new Amenity(request.Name, request.Description, request.Cost);

            amenityRepository.Add(amenity);
            amenityRepository.SaveChanges();

            return AmenityResponse.From(amenity);
        }

        public IReadOnlyList<AmenityResponse> GetAll() =>
            amenityRepository.GetAll().Select(AmenityResponse.From).ToList();

        public AmenityResponse? GetById(Guid id)
        {
            var amenity = amenityRepository.GetById(id);

            return amenity == null ? null : AmenityResponse.From(amenity);
        }

        public bool Delete(Guid id)
        {
            var amenity = amenityRepository.GetById(id);

            if (amenity == null)
            {
                return false;
            }

            amenityRepository.Remove(amenity);
            amenityRepository.SaveChanges();

            return true;
        }
    }
}
