namespace GestionComplejo2C.Domain.Entities
{
    public class Cancha
    {
        private static int siguienteId = 1;
        private readonly List<Reserva> reservas = new List<Reserva>();

        public int Id { get; }
        public string Deporte { get; }
        public string TipoPiso { get; }
        public int JugadoresMax { get; }
        public decimal PrecioPorHora { get; private set; }

        public decimal Recaudacion => reservas.Where(r => !r.Cancelada).Sum(r => r.Importe);
        public int ReservasActivas => reservas.Count(r => !r.Cancelada);

        public Cancha(string deporte, string tipoPiso, int jugadoresMax, decimal precioPorHora)
        {
            if (string.IsNullOrWhiteSpace(deporte))
                throw new ArgumentException("El deporte es obligatorio.", nameof(deporte));

            if (string.IsNullOrWhiteSpace(tipoPiso))
                throw new ArgumentException("El tipo de piso es obligatorio.", nameof(tipoPiso));

            if (jugadoresMax <= 0)
                throw new ArgumentOutOfRangeException(nameof(jugadoresMax), "Debe haber al menos un jugador.");

            if (precioPorHora <= 0)
                throw new ArgumentOutOfRangeException(nameof(precioPorHora), "El precio debe ser mayor a cero.");

            Id = siguienteId++;
            Deporte = deporte;
            TipoPiso = tipoPiso;
            JugadoresMax = jugadoresMax;
            PrecioPorHora = precioPorHora;
        }

        public Reserva Reservar(string cliente, DateTime inicio, int horas)
        {
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("La reserva necesita un cliente.", nameof(cliente));

            if (horas <= 0)
                throw new ArgumentOutOfRangeException(nameof(horas), "La reserva debe durar al menos una hora.");

            if (!EstaLibre(inicio, horas))
                throw new InvalidOperationException("La cancha ya está reservada en ese horario.");

            var reserva = new Reserva(cliente, inicio, horas, PrecioPorHora * horas);
            reservas.Add(reserva);
            return reserva;
        }

        public void Cancelar(Guid idReserva)
        {
            var reserva = ObtenerReserva(idReserva)
                ?? throw new InvalidOperationException($"No existe la reserva {idReserva}.");

            if (reserva.Cancelada)
                throw new InvalidOperationException("La reserva ya estaba cancelada.");

            reserva.Cancelar();
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                throw new ArgumentOutOfRangeException(nameof(nuevoPrecio), "El precio debe ser mayor a cero.");

            PrecioPorHora = nuevoPrecio;
        }

        public bool EstaLibre(DateTime inicio, int horas)
        {
            var fin = inicio.AddHours(horas);
            return !reservas.Any(r => !r.Cancelada && inicio < r.Fin && fin > r.Inicio);
        }

        public Reserva? ObtenerReserva(Guid idReserva) => reservas.FirstOrDefault(r => r.Id == idReserva);

        public IReadOnlyList<Reserva> VerHistorial() => reservas.AsReadOnly();
    }
}
