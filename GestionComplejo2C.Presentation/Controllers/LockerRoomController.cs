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
        public async Task<ActionResult<LockerRoomResponse>> Create([FromBody] CreateLockerRoomRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var lockerRoom = await lockerRoomService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { id = lockerRoom.Id }, lockerRoom);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LockerRoomResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var lockerRooms = await lockerRoomService.GetAllAsync(cancellationToken);

            if (!lockerRooms.Any())
            {
                return NotFound("No hay vestuarios registrados.");
            }

            return Ok(lockerRooms);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LockerRoomResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var lockerRoom = await lockerRoomService.GetByIdAsync(id, cancellationToken);

            if (lockerRoom == null)
            {
                return NotFound($"No existe un vestuario con el id {id}.");
            }

            return Ok(lockerRoom);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            if (!await lockerRoomService.DeleteAsync(id, cancellationToken))
            {
                return NotFound($"No existe un vestuario con el id {id}.");
            }

            return NoContent();
        }
    }
}
