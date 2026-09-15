namespace GestionComplejo2C.Domain.Entities
{
    public class Vestuario
    {
        public Guid Id { get; private set; }
        public bool Disponible { get; private set; }
        public int Duchas { get; private set; }
        public int Capacidad { get; private set; }

        public Cancha? Cancha { get; private set; }

        private Vestuario()
        {
        }

        public Vestuario(bool disponible, int duchas, int capacidad)
        {
            if (duchas < 0)
                throw new ArgumentOutOfRangeException(nameof(duchas), "La cantidad de duchas no puede ser negativa.");

            if (capacidad <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacidad), "La capacidad debe ser mayor a cero.");

            Id = Guid.NewGuid();
            Disponible = disponible;
            Duchas = duchas;
            Capacidad = capacidad;
        }
    }
}
