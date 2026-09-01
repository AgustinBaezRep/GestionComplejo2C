using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/cancha/{canchaId}/reservas")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        private readonly IReservaService reservaService;

        public ReservaController(IReservaService reservaService)
        {
            this.reservaService = reservaService;
        }

        [HttpPost]
        public ActionResult<Reserva> Create([FromRoute] int canchaId, [FromBody] CrearReservaRequest request)
        {
            try
            {
                var reserva = reservaService.Crear(canchaId, request);

                return CreatedAtAction(nameof(GetById), new { canchaId, id = reserva.Id }, reserva);
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
        public ActionResult<IReadOnlyList<Reserva>> GetAll([FromRoute] int canchaId)
        {
            try
            {
                return Ok(reservaService.ObtenerTodas(canchaId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Reserva> GetById([FromRoute] int canchaId, [FromRoute] Guid id)
        {
            try
            {
                var reserva = reservaService.ObtenerPorId(canchaId, id);

                if (reserva == null)
                {
                    return NotFound($"There is no booking that match with the id {id}");
                }

                return Ok(reserva);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int canchaId, [FromRoute] Guid id)
        {
            try
            {
                if (!reservaService.Cancelar(canchaId, id))
                {
                    return NotFound($"There is no booking that match with the id {id}");
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
