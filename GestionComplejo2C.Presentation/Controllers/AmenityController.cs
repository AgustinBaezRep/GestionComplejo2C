using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Policies.AdminsOnly)]
    [ApiController]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityService amenityService;

        public AmenityController(IAmenityService amenityService)
        {
            this.amenityService = amenityService;
        }

        [HttpPost]
        public ActionResult<AmenityResponse> Create([FromBody] CreateAmenityRequest request)
        {
            try
            {
                var amenity = amenityService.Create(request);

                return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<AmenityResponse>> GetAll()
        {
            var amenities = amenityService.GetAll();

            if (!amenities.Any())
            {
                return NotFound("No hay servicios registrados.");
            }

            return Ok(amenities);
        }

        [HttpGet("{id}")]
        public ActionResult<AmenityResponse> GetById([FromRoute] Guid id)
        {
            var amenity = amenityService.GetById(id);

            if (amenity == null)
            {
                return NotFound($"No existe un servicio con el id {id}.");
            }

            return Ok(amenity);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            if (!amenityService.Delete(id))
            {
                return NotFound($"No existe un servicio con el id {id}.");
            }

            return NoContent();
        }
    }
}
