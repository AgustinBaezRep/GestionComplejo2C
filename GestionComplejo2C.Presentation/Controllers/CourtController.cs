using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.AdminsOnly)]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService courtService;

        public CourtController(ICourtService courtService)
        {
            this.courtService = courtService;
        }

        [HttpPost]
        public async Task<ActionResult<CourtResponse>> Create([FromBody] CreateCourtRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var court = await courtService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { id = court.Id }, court);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CourtResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var courts = await courtService.GetAllAsync(cancellationToken);

            if (!courts.Any())
            {
                return NotFound("No hay canchas registradas.");
            }

            return Ok(courts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourtResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var court = await courtService.GetByIdAsync(id, cancellationToken);

            if (court == null)
            {
                return NotFound($"No existe una cancha con el id {id}.");
            }

            return Ok(court);
        }

        [HttpPatch("{id}/price")]
        public async Task<ActionResult<CourtResponse>> UpdatePrice([FromRoute] Guid id, [FromBody] UpdatePriceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var court = await courtService.UpdatePriceAsync(id, request, cancellationToken);

                if (court == null)
                {
                    return NotFound($"No existe una cancha con el id {id}.");
                }

                return Ok(court);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if (!await courtService.DeleteAsync(id, cancellationToken))
                {
                    return NotFound($"No existe una cancha con el id {id}.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}/locker-room/{lockerRoomId}")]
        public async Task<ActionResult<CourtResponse>> AssignLockerRoom([FromRoute] Guid id, [FromRoute] Guid lockerRoomId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await courtService.AssignLockerRoomAsync(id, lockerRoomId, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}/locker-room")]
        public async Task<ActionResult<CourtResponse>> RemoveLockerRoom([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await courtService.RemoveLockerRoomAsync(id, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/amenities/{amenityId}")]
        public async Task<ActionResult<CourtResponse>> AddAmenity([FromRoute] Guid id, [FromRoute] Guid amenityId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await courtService.AddAmenityAsync(id, amenityId, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}/amenities/{amenityId}")]
        public async Task<ActionResult<CourtResponse>> RemoveAmenity([FromRoute] Guid id, [FromRoute] Guid amenityId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await courtService.RemoveAmenityAsync(id, amenityId, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
