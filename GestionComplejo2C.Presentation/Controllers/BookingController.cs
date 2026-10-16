using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/court/{courtId}/bookings")]
    [Authorize(Policy = Policies.Bookings)]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService bookingService;

        public BookingController(IBookingService bookingService)
        {
            this.bookingService = bookingService;
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponse>> Create([FromRoute] Guid courtId, [FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var booking = await bookingService.CreateAsync(courtId, request, cancellationToken);

                return CreatedAtAction(nameof(GetById), new { courtId, id = booking.Id }, booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "No se pudo consultar el servicio del clima.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<BookingResponse>>> GetAll([FromRoute] Guid courtId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await bookingService.GetAllAsync(courtId, cancellationToken));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponse>> GetById([FromRoute] Guid courtId, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var booking = await bookingService.GetByIdAsync(courtId, id, cancellationToken);

                if (booking == null)
                {
                    return NotFound($"No existe una reserva con el id {id}.");
                }

                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid courtId, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            try
            {
                if (!await bookingService.CancelAsync(courtId, id, cancellationToken))
                {
                    return NotFound($"No existe una reserva con el id {id}.");
                }

                return NoContent();
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
