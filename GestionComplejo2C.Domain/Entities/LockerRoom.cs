namespace GestionComplejo2C.Domain.Entities
{
    public class LockerRoom
    {
        public Guid Id { get; private set; }
        public bool Available { get; private set; }
        public int Showers { get; private set; }
        public int Capacity { get; private set; }

        public Court? Court { get; private set; }

        private LockerRoom()
        {
        }

        public LockerRoom(bool available, int showers, int capacity)
        {
            if (showers < 0)
                throw new ArgumentOutOfRangeException(nameof(showers), "La cantidad de duchas no puede ser negativa.");

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "La capacidad debe ser mayor a cero.");

            Id = Guid.NewGuid();
            Available = available;
            Showers = showers;
            Capacity = capacity;
        }
    }
}
