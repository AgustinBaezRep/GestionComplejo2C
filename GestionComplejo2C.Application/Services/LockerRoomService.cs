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

        public LockerRoomResponse Create(CreateLockerRoomRequest request)
        {
            var lockerRoom = new LockerRoom(request.Available, request.Showers, request.Capacity);

            lockerRoomRepository.Add(lockerRoom);
            lockerRoomRepository.SaveChanges();

            return LockerRoomResponse.From(lockerRoom);
        }

        public IReadOnlyList<LockerRoomResponse> GetAll() =>
            lockerRoomRepository.GetAll().Select(LockerRoomResponse.From).ToList();

        public LockerRoomResponse? GetById(Guid id)
        {
            var lockerRoom = lockerRoomRepository.GetById(id);

            return lockerRoom == null ? null : LockerRoomResponse.From(lockerRoom);
        }

        public bool Delete(Guid id)
        {
            var lockerRoom = lockerRoomRepository.GetById(id);

            if (lockerRoom == null)
            {
                return false;
            }

            lockerRoomRepository.Remove(lockerRoom);
            lockerRoomRepository.SaveChanges();

            return true;
        }
    }
}
