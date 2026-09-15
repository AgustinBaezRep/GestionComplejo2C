namespace GestionComplejo2C.Domain.Entities
{
    public class Servicio
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public string Descripcion { get; private set; } = string.Empty;
        public decimal Costo { get; private set; }

        public List<Cancha> Canchas { get; private set; } = new List<Cancha>();

        private Servicio()
        {
        }

        public Servicio(string nombre, string descripcion, decimal costo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            if (costo < 0)
                throw new ArgumentOutOfRangeException(nameof(costo), "El costo no puede ser negativo.");

            Id = Guid.NewGuid();
            Nombre = nombre;
            Descripcion = descripcion ?? string.Empty;
            Costo = costo;
        }
    }
}
