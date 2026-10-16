using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class LockerRoomService : ILockerRoomService
    {
        private readonly ILockerRoomRepository lockerRoomRepository;

        public LockerRoomService(ILockerRoomRepository lockerRoomRepository)
        {
            this.lockerRoomRepository = lockerRoomRepository;
        }

        public async Task<LockerRoomResponse> CreateAsync(CreateLockerRoomRequest request, CancellationToken cancellationToken = default)
        {
            var lockerRoom = new LockerRoom(request.Available, request.Showers, request.Capacity);

            lockerRoomRepository.Add(lockerRoom);
            await lockerRoomRepository.SaveChangesAsync(cancellationToken);

            return LockerRoomResponse.From(lockerRoom);
        }

        public async Task<IReadOnlyList<LockerRoomResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var lockerRooms = await lockerRoomRepository.GetAllAsync(cancellationToken);

            return lockerRooms.Select(LockerRoomResponse.From).ToList();
        }

        public async Task<LockerRoomResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var lockerRoom = await lockerRoomRepository.GetByIdAsync(id, cancellationToken);

            return lockerRoom == null ? null : LockerRoomResponse.From(lockerRoom);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var lockerRoom = await lockerRoomRepository.GetByIdAsync(id, cancellationToken);

            if (lockerRoom == null)
            {
                return false;
            }

            lockerRoomRepository.Remove(lockerRoom);
            await lockerRoomRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
