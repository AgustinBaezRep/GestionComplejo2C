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
        public async Task<ActionResult<AmenityResponse>> Create([FromBody] CreateAmenityRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var amenity = await amenityService.CreateAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenity);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AmenityResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var amenities = await amenityService.GetAllAsync(cancellationToken);

            if (!amenities.Any())
            {
                return NotFound("No hay servicios registrados.");
            }

            return Ok(amenities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AmenityResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var amenity = await amenityService.GetByIdAsync(id, cancellationToken);

            if (amenity == null)
            {
                return NotFound($"No existe un servicio con el id {id}.");
            }

            return Ok(amenity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            if (!await amenityService.DeleteAsync(id, cancellationToken))
            {
                return NotFound($"No existe un servicio con el id {id}.");
            }

            return NoContent();
        }
    }
}
