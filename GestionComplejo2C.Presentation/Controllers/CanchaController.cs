using GestionComplejo2C.Presentation.Data;
using GestionComplejo2C.Presentation.DTOs;
using GestionComplejo2C.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanchaController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Cancha> Create([FromBody] CrearCanchaRequest request)
        {
            try
            {
                var cancha = new Cancha(request.Deporte, request.TipoPiso, request.JugadoresMax, request.PrecioPorHora);

                RepositorioCanchas.Agregar(cancha);

                return CreatedAtAction(nameof(GetById), new { id = cancha.Id }, cancha);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<Cancha>> GetAll()
        {
            var canchas = RepositorioCanchas.ObtenerTodas();

            if (!canchas.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(canchas);
        }

        [HttpGet("{id}")]
        public ActionResult<Cancha> GetById([FromRoute] int id)
        {
            var cancha = RepositorioCanchas.ObtenerPorId(id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(cancha);
        }

        [HttpPatch("{id}/precio")]
        public ActionResult<Cancha> UpdatePrecio([FromRoute] int id, [FromBody] ActualizarPrecioRequest request)
        {
            var cancha = RepositorioCanchas.ObtenerPorId(id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            try
            {
                cancha.ActualizarPrecio(request.PrecioPorHora);

                return Ok(cancha);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var cancha = RepositorioCanchas.ObtenerPorId(id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            if (cancha.ReservasActivas > 0)
            {
                return Conflict($"The court {id} has active bookings");
            }

            if (!RepositorioCanchas.Eliminar(cancha))
            {
                return Conflict($"Problem to delete the item {id}");
            }

            return NoContent();
        }
    }
}
