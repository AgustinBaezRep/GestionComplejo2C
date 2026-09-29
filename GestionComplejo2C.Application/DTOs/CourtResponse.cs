using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record CourtResponse(
        Guid Id,
        string Sport,
        string SurfaceType,
        int MaxPlayers,
        decimal PricePerHour,
        decimal Revenue,
        int ActiveBookings,
        LockerRoomResponse? LockerRoom,
        IReadOnlyList<AmenityResponse> Amenities)
    {
        public static CourtResponse From(Court court) => new(
            court.Id,
            court.Sport,
            court.SurfaceType,
            court.MaxPlayers,
            court.PricePerHour,
            court.Revenue,
            court.ActiveBookings,
            court.LockerRoom is null ? null : LockerRoomResponse.From(court.LockerRoom),
            court.Amenities.Select(AmenityResponse.From).ToList());
    }
}
