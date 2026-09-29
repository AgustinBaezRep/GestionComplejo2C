using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface ILockerRoomService
    {
        LockerRoomResponse Create(CreateLockerRoomRequest request);

        IReadOnlyList<LockerRoomResponse> GetAll();

        LockerRoomResponse? GetById(Guid id);

        bool Delete(Guid id);
    }
}
