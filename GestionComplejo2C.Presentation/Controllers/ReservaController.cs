using GestionComplejo2C.Presentation.Data;
using GestionComplejo2C.Presentation.DTOs;
using GestionComplejo2C.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/cancha/{canchaId}/reservas")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Reserva> Create([FromRoute] int canchaId, [FromBody] CrearReservaRequest request)
        {
            var cancha = RepositorioCanchas.ObtenerPorId(canchaId);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {canchaId}");
            }

            try
            {
                var reserva = cancha.Reservar(request.Cliente, request.Inicio, request.Horas);

                return CreatedAtAction(nameof(GetById), new { canchaId, id = reserva.Id }, reserva);
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
            var cancha = RepositorioCanchas.ObtenerPorId(canchaId);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {canchaId}");
            }

            return Ok(cancha.VerHistorial());
        }

        [HttpGet("{id}")]
        public ActionResult<Reserva> GetById([FromRoute] int canchaId, [FromRoute] Guid id)
        {
            var cancha = RepositorioCanchas.ObtenerPorId(canchaId);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {canchaId}");
            }

            var reserva = cancha.ObtenerReserva(id);

            if (reserva == null)
            {
                return NotFound($"There is no booking that match with the id {id}");
            }

            return Ok(reserva);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int canchaId, [FromRoute] Guid id)
        {
            var cancha = RepositorioCanchas.ObtenerPorId(canchaId);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {canchaId}");
            }

            if (cancha.ObtenerReserva(id) == null)
            {
                return NotFound($"There is no booking that match with the id {id}");
            }

            try
            {
                cancha.Cancelar(id);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
