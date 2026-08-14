using GestionComplejo2C.Presentation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CanchaController : ControllerBase
    {
        private static readonly List<Cancha> Canchas = new List<Cancha>();

        [HttpPost]
        public ActionResult Create([FromBody] Cancha cancha)
        {
            // crear el objeto cancha
            var objetoCancha = new Cancha();

            objetoCancha.Id = cancha.Id;
            objetoCancha.Deporte = cancha.Deporte;
            objetoCancha.TipoPiso = cancha.TipoPiso;
            objetoCancha.JugadoresMax = cancha.JugadoresMax;
            objetoCancha.Precio = cancha.Precio;

            // guardar el objeto cancha dentro de la tabla
            Canchas.Add(cancha);

            // devolver el status code
            return Created();
        }

        [HttpGet]
        public ActionResult<List<Cancha>> GetAll()
        {
            if (!Canchas.Any())
            {
                return NotFound("No elements within the list");
            }

            return Ok(Canchas);
        }

        [HttpGet("{id}")]
        public ActionResult<Cancha> GetById([FromRoute] int id)
        {
            var cancha = Canchas.FirstOrDefault(x => x.Id == id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            return Ok(cancha);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            var cancha = Canchas.FirstOrDefault(x => x.Id == id);

            if (cancha == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            if (!Canchas.Remove(cancha))
            {
                return Conflict($"Problem to delete the item {id}");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult<Cancha> PartialUpdate([FromRoute] int id, [FromBody] Cancha cancha)
        {
            var canchaFound = Canchas.FirstOrDefault(x => x.Id == id);

            if (canchaFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            canchaFound.Deporte = cancha.Deporte ?? canchaFound.Deporte;
            canchaFound.JugadoresMax = cancha.JugadoresMax ?? canchaFound.JugadoresMax;
            canchaFound.TipoPiso = cancha.TipoPiso ?? canchaFound.TipoPiso;
            canchaFound.Precio = cancha.Precio ?? canchaFound.Precio;

            return Ok(canchaFound);
        }

        [HttpPut("{id}")]
        public ActionResult<Cancha> Update([FromRoute] int id, [FromBody] Cancha cancha)
        {
            var canchaFound = Canchas.FirstOrDefault(x => x.Id == id);

            if (canchaFound == null)
            {
                return NotFound($"There is no element that match with the id {id}");
            }

            canchaFound.Deporte = cancha.Deporte;
            canchaFound.JugadoresMax = cancha.JugadoresMax;
            canchaFound.TipoPiso = cancha.TipoPiso;
            canchaFound.Precio = cancha.Precio;

            return Ok(canchaFound);
        }
    }
}
