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
        public ActionResult<BookingResponse> Create([FromRoute] Guid courtId, [FromBody] CreateBookingRequest request)
        {
            try
            {
                var booking = bookingService.Create(courtId, request);

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
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<BookingResponse>> GetAll([FromRoute] Guid courtId)
        {
            try
            {
                return Ok(bookingService.GetAll(courtId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<BookingResponse> GetById([FromRoute] Guid courtId, [FromRoute] Guid id)
        {
            try
            {
                var booking = bookingService.GetById(courtId, id);

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
        public ActionResult Delete([FromRoute] Guid courtId, [FromRoute] Guid id)
        {
            try
            {
                if (!bookingService.Cancel(courtId, id))
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
