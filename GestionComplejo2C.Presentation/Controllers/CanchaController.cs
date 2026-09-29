using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Politicas.SoloAdministradores)]
    [ApiController]
    public class CanchaController : ControllerBase
    {
        private readonly ICanchaService canchaService;

        public CanchaController(ICanchaService canchaService)
        {
            this.canchaService = canchaService;
        }

        [HttpPost]
        public ActionResult<CanchaResponse> Create([FromBody] CrearCanchaRequest request)
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
        public ActionResult<IReadOnlyList<CanchaResponse>> GetAll()
        {
            var canchas = canchaService.ObtenerTodas();

            if (!canchas.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(canchas);
        }

        [HttpGet("{id}")]
        public ActionResult<CanchaResponse> GetById([FromRoute] Guid id)
        {
            var cancha = canchaService.ObtenerPorId(id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(cancha);
        }

        [HttpPatch("{id}/precio")]
        public ActionResult<CanchaResponse> UpdatePrecio([FromRoute] Guid id, [FromBody] ActualizarPrecioRequest request)
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
        public ActionResult Delete([FromRoute] Guid id)
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

        [HttpPut("{id}/vestuario/{vestuarioId}")]
        public ActionResult<CanchaResponse> AssignVestuario([FromRoute] Guid id, [FromRoute] Guid vestuarioId)
        {
            try
            {
                return Ok(canchaService.AsignarVestuario(id, vestuarioId));
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

        [HttpDelete("{id}/vestuario")]
        public ActionResult<CanchaResponse> RemoveVestuario([FromRoute] Guid id)
        {
            try
            {
                return Ok(canchaService.QuitarVestuario(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/servicios/{servicioId}")]
        public ActionResult<CanchaResponse> AddServicio([FromRoute] Guid id, [FromRoute] Guid servicioId)
        {
            try
            {
                return Ok(canchaService.AgregarServicio(id, servicioId));
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

        [HttpDelete("{id}/servicios/{servicioId}")]
        public ActionResult<CanchaResponse> RemoveServicio([FromRoute] Guid id, [FromRoute] Guid servicioId)
        {
            try
            {
                return Ok(canchaService.QuitarServicio(id, servicioId));
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
