using Microsoft.AspNetCore.Mvc;

namespace GestionComplejo2C.Presentation.Models
{
    public class Cancha
    {
        public int Id { get; set; }
        public string? Deporte { get; set; }
        public int? JugadoresMax { get; set; }
        public string? TipoPiso { get; set; } = string.Empty;
        public double? Precio { get; set; }
    }
}
