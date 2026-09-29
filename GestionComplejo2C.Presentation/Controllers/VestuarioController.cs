using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class VestuarioController : ControllerBase
    {
        private readonly IVestuarioService vestuarioService;

        public VestuarioController(IVestuarioService vestuarioService)
        {
            this.vestuarioService = vestuarioService;
        }

        [HttpPost]
        public ActionResult<VestuarioResponse> Create([FromBody] CrearVestuarioRequest request)
        {
            try
            {
                var vestuario = vestuarioService.Crear(request);

                return CreatedAtAction(nameof(GetById), new { id = vestuario.Id }, vestuario);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IReadOnlyList<VestuarioResponse>> GetAll()
        {
            var vestuarios = vestuarioService.ObtenerTodos();

            if (!vestuarios.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(vestuarios);
        }

        [HttpGet("{id}")]
        public ActionResult<VestuarioResponse> GetById([FromRoute] Guid id)
        {
            var vestuario = vestuarioService.ObtenerPorId(id);

            if (vestuario == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(vestuario);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] Guid id)
        {
            if (!vestuarioService.Eliminar(id))
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return NoContent();
        }
    }
}
