using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record LockerRoomResponse(
        Guid Id,
        bool Available,
        int Showers,
        int Capacity,
        Guid? CourtId)
    {
        public static LockerRoomResponse From(LockerRoom lockerRoom) => new(
            lockerRoom.Id,
            lockerRoom.Available,
            lockerRoom.Showers,
            lockerRoom.Capacity,
            lockerRoom.Court?.Id);
    }
}
