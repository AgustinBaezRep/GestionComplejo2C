namespace GestionComplejo2C.Domain.Entities
{
    public class Amenity
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public decimal Cost { get; private set; }

        public List<Court> Courts { get; private set; } = new List<Court>();

        private Amenity()
        {
        }

        public Amenity(string name, string description, decimal cost)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre es obligatorio.", nameof(name));

            if (cost < 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "El costo no puede ser negativo.");

            Id = Guid.NewGuid();
            Name = name;
            Description = description ?? string.Empty;
            Cost = cost;
        }
    }
}
