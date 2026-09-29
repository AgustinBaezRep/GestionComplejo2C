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
        public ActionResult<CourtResponse> Create([FromBody] CreateCourtRequest request)
        {
            try
            {
                var court = courtService.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = court.Id }, court);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<CourtResponse>> GetAll()
        {
            var courts = courtService.GetAll();

            if (!courts.Any())
            {
                return NotFound("No hay canchas registradas.");
            }

            return Ok(courts);
        }

        [HttpGet("{id}")]
        public ActionResult<CourtResponse> GetById([FromRoute] Guid id)
        {
            var court = courtService.GetById(id);

            if (court == null)
            {
                return NotFound($"No existe una cancha con el id {id}.");
            }

            return Ok(court);
        }

        [HttpPatch("{id}/price")]
        public ActionResult<CourtResponse> UpdatePrice([FromRoute] Guid id, [FromBody] UpdatePriceRequest request)
        {
            try
            {
                var court = courtService.UpdatePrice(id, request);

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
        public ActionResult Delete([FromRoute] Guid id)
        {
            try
            {
                if (!courtService.Delete(id))
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
        public ActionResult<CourtResponse> AssignLockerRoom([FromRoute] Guid id, [FromRoute] Guid lockerRoomId)
        {
            try
            {
                return Ok(courtService.AssignLockerRoom(id, lockerRoomId));
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
        public ActionResult<CourtResponse> RemoveLockerRoom([FromRoute] Guid id)
        {
            try
            {
                return Ok(courtService.RemoveLockerRoom(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/amenities/{amenityId}")]
        public ActionResult<CourtResponse> AddAmenity([FromRoute] Guid id, [FromRoute] Guid amenityId)
        {
            try
            {
                return Ok(courtService.AddAmenity(id, amenityId));
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
        public ActionResult<CourtResponse> RemoveAmenity([FromRoute] Guid id, [FromRoute] Guid amenityId)
        {
            try
            {
                return Ok(courtService.RemoveAmenity(id, amenityId));
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
