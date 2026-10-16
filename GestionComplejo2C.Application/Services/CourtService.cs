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

        public async Task<CourtResponse> CreateAsync(CreateCourtRequest request, CancellationToken cancellationToken = default)
        {
            var court = new Court(request.Sport, request.SurfaceType, request.MaxPlayers, request.PricePerHour);

            courtRepository.Add(court);
            await courtRepository.SaveChangesAsync(cancellationToken);

            return CourtResponse.From(court);
        }

        public async Task<IReadOnlyList<CourtResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var courts = await courtRepository.GetAllAsync(cancellationToken);

            return courts.Select(CourtResponse.From).ToList();
        }

        public async Task<CourtResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var court = await courtRepository.GetByIdAsync(id, cancellationToken);

            return court == null ? null : CourtResponse.From(court);
        }

        public async Task<CourtResponse?> UpdatePriceAsync(Guid id, UpdatePriceRequest request, CancellationToken cancellationToken = default)
        {
            var court = await courtRepository.GetByIdAsync(id, cancellationToken);

            if (court == null)
            {
                return null;
            }

            court.UpdatePrice(request.PricePerHour);

            await courtRepository.SaveChangesAsync(cancellationToken);

            return CourtResponse.From(court);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var court = await courtRepository.GetByIdAsync(id, cancellationToken);

            if (court == null)
            {
                return false;
            }

            if (court.ActiveBookings > 0)
            {
                throw new InvalidOperationException($"La cancha {id} tiene reservas activas.");
            }

            courtRepository.Remove(court);
            await courtRepository.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<CourtResponse> AssignLockerRoomAsync(Guid courtId, Guid lockerRoomId, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            var lockerRoom = await lockerRoomRepository.GetByIdAsync(lockerRoomId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe un vestuario con el id {lockerRoomId}.");

            if (lockerRoom.Court != null && lockerRoom.Court.Id != courtId)
            {
                throw new InvalidOperationException($"El vestuario {lockerRoomId} ya está asignado a otra cancha.");
            }

            court.AssignLockerRoom(lockerRoom);

            await courtRepository.SaveChangesAsync(cancellationToken);

            return CourtResponse.From(court);
        }

        public async Task<CourtResponse> RemoveLockerRoomAsync(Guid courtId, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            court.RemoveLockerRoom();

            await courtRepository.SaveChangesAsync(cancellationToken);

            return CourtResponse.From(court);
        }

        public async Task<CourtResponse> AddAmenityAsync(Guid courtId, Guid amenityId, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            var amenity = await amenityRepository.GetByIdAsync(amenityId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe un servicio con el id {amenityId}.");

            court.AddAmenity(amenity);

            await courtRepository.SaveChangesAsync(cancellationToken);

            return CourtResponse.From(court);
        }

        public async Task<CourtResponse> RemoveAmenityAsync(Guid courtId, Guid amenityId, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            court.RemoveAmenity(amenityId);

            await courtRepository.SaveChangesAsync(cancellationToken);

            return CourtResponse.From(court);
        }

        private async Task<Court> GetCourtAsync(Guid courtId, CancellationToken cancellationToken) =>
            await courtRepository.GetByIdAsync(courtId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe una cancha con el id {courtId}.");
    }
}
