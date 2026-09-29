using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface ICourtService
    {
        CourtResponse Create(CreateCourtRequest request);

        IReadOnlyList<CourtResponse> GetAll();

        CourtResponse? GetById(Guid id);

        CourtResponse? UpdatePrice(Guid id, UpdatePriceRequest request);

        bool Delete(Guid id);

        CourtResponse AssignLockerRoom(Guid courtId, Guid lockerRoomId);

        CourtResponse RemoveLockerRoom(Guid courtId);

        CourtResponse AddAmenity(Guid courtId, Guid amenityId);

        CourtResponse RemoveAmenity(Guid courtId, Guid amenityId);
    }
}
