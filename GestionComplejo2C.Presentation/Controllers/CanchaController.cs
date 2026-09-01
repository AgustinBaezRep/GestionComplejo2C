using GestionComplejo2C.Presentation.DTOs;
using GestionComplejo2C.Presentation.Models;
using GestionComplejo2C.Presentation.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanchaController : ControllerBase
    {
        private readonly ICanchaService canchaService;

        public CanchaController(ICanchaService canchaService)
        {
            this.canchaService = canchaService;
        }

        [HttpPost]
        public ActionResult<Cancha> Create([FromBody] CrearCanchaRequest request)
        {
            try
            {
                var cancha = canchaService.Crear(request);

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
            var canchas = canchaService.ObtenerTodas();

            if (!canchas.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(canchas);
        }

        [HttpGet("{id}")]
        public ActionResult<Cancha> GetById([FromRoute] int id)
        {
            var cancha = canchaService.ObtenerPorId(id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(cancha);
        }

        [HttpPatch("{id}/precio")]
        public ActionResult<Cancha> UpdatePrecio([FromRoute] int id, [FromBody] ActualizarPrecioRequest request)
        {
            try
            {
                var cancha = canchaService.ActualizarPrecio(id, request);

                if (cancha == null)
                {
                    return NotFound($"There is no element that match with the id {id}");
                }

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
            try
            {
                if (!canchaService.Eliminar(id))
                {
                    return NotFound($"There is no element that match with the id {id}");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
