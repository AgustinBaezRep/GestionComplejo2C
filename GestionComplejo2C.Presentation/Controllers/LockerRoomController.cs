using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.AdminsOnly)]
    [ApiController]
    public class LockerRoomController : ControllerBase
    {
        private readonly ILockerRoomService lockerRoomService;

        public LockerRoomController(ILockerRoomService lockerRoomService)
        {
            this.lockerRoomService = lockerRoomService;
        }

        [HttpPost]
        public ActionResult<LockerRoomResponse> Create([FromBody] CreateLockerRoomRequest request)
        {
            try
            {
                var lockerRoom = lockerRoomService.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = lockerRoom.Id }, lockerRoom);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<LockerRoomResponse>> GetAll()
        {
            var lockerRooms = lockerRoomService.GetAll();

            if (!lockerRooms.Any())
            {
                return NotFound("No hay vestuarios registrados.");
            }

            return Ok(lockerRooms);
        }

        [HttpGet("{id}")]
        public ActionResult<LockerRoomResponse> GetById([FromRoute] Guid id)
        {
            var lockerRoom = lockerRoomService.GetById(id);

            if (lockerRoom == null)
            {
                return NotFound($"No existe un vestuario con el id {id}.");
            }

            return Ok(lockerRoom);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            if (!lockerRoomService.Delete(id))
            {
                return NotFound($"No existe un vestuario con el id {id}.");
            }

            return NoContent();
        }
    }
}
