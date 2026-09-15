using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicioController : ControllerBase
    {
        private readonly IServicioService servicioService;

        public ServicioController(IServicioService servicioService)
        {
            this.servicioService = servicioService;
        }

        [HttpPost]
        public ActionResult<ServicioResponse> Create([FromBody] CrearServicioRequest request)
        {
            try
            {
                var servicio = servicioService.Crear(request);

                return CreatedAtAction(nameof(GetById), new { id = servicio.Id }, servicio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<ServicioResponse>> GetAll()
        {
            var servicios = servicioService.ObtenerTodos();

            if (!servicios.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(servicios);
        }

        [HttpGet("{id}")]
        public ActionResult<ServicioResponse> GetById([FromRoute] Guid id)
        {
            var servicio = servicioService.ObtenerPorId(id);

            if (servicio == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(servicio);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            if (!servicioService.Eliminar(id))
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return NoContent();
        }
    }
}
