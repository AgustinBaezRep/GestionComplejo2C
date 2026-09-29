using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository courtRepository;
        private readonly ILockerRoomRepository lockerRoomRepository;
        private readonly IAmenityRepository amenityRepository;

        public CourtService(
            ICourtRepository courtRepository,
            ILockerRoomRepository lockerRoomRepository,
            IAmenityRepository amenityRepository)
        {
            this.courtRepository = courtRepository;
            this.lockerRoomRepository = lockerRoomRepository;
            this.amenityRepository = amenityRepository;
        }

        public CourtResponse Create(CreateCourtRequest request)
        {
            var court = new Court(request.Sport, request.SurfaceType, request.MaxPlayers, request.PricePerHour);

            courtRepository.Add(court);
            courtRepository.SaveChanges();

            return CourtResponse.From(court);
        }

        public IReadOnlyList<CourtResponse> GetAll() =>
            courtRepository.GetAll().Select(CourtResponse.From).ToList();

        public CourtResponse? GetById(Guid id)
        {
            var court = courtRepository.GetById(id);

            return court == null ? null : CourtResponse.From(court);
        }

        public CourtResponse? UpdatePrice(Guid id, UpdatePriceRequest request)
        {
            var court = courtRepository.GetById(id);

            if (court == null)
            {
                return null;
            }

            court.UpdatePrice(request.PricePerHour);

            courtRepository.SaveChanges();

            return CourtResponse.From(court);
        }

        public bool Delete(Guid id)
        {
            var court = courtRepository.GetById(id);

            if (court == null)
            {
                return false;
            }

            if (court.ActiveBookings > 0)
            {
                throw new InvalidOperationException($"La cancha {id} tiene reservas activas.");
            }

            courtRepository.Remove(court);
            courtRepository.SaveChanges();

            return true;
        }

        public CourtResponse AssignLockerRoom(Guid courtId, Guid lockerRoomId)
        {
            var court = GetCourt(courtId);

            var lockerRoom = lockerRoomRepository.GetById(lockerRoomId)
                ?? throw new KeyNotFoundException($"No existe un vestuario con el id {lockerRoomId}.");

            if (lockerRoom.Court != null && lockerRoom.Court.Id != courtId)
            {
                throw new InvalidOperationException($"El vestuario {lockerRoomId} ya está asignado a otra cancha.");
            }

            court.AssignLockerRoom(lockerRoom);

            courtRepository.SaveChanges();

            return CourtResponse.From(court);
        }

        public CourtResponse RemoveLockerRoom(Guid courtId)
        {
            var court = GetCourt(courtId);

            court.RemoveLockerRoom();

            courtRepository.SaveChanges();

            return CourtResponse.From(court);
        }

        public CourtResponse AddAmenity(Guid courtId, Guid amenityId)
        {
            var court = GetCourt(courtId);

            var amenity = amenityRepository.GetById(amenityId)
                ?? throw new KeyNotFoundException($"No existe un servicio con el id {amenityId}.");

            court.AddAmenity(amenity);

            courtRepository.SaveChanges();

            return CourtResponse.From(court);
        }

        public CourtResponse RemoveAmenity(Guid courtId, Guid amenityId)
        {
            var court = GetCourt(courtId);

            court.RemoveAmenity(amenityId);

            courtRepository.SaveChanges();

            return CourtResponse.From(court);
        }

        private Court GetCourt(Guid courtId) =>
            courtRepository.GetById(courtId)
                ?? throw new KeyNotFoundException($"No existe una cancha con el id {courtId}.");
    }
}
