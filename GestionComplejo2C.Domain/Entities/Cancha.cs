namespace GestionComplejo2C.Domain.Entities
{
    public class Cancha
    {
        public Guid Id { get; private set; }
        public string Deporte { get; private set; } = string.Empty;
        public string TipoPiso { get; private set; } = string.Empty;
        public int JugadoresMax { get; private set; }
        public decimal PrecioPorHora { get; private set; }

        public Guid? VestuarioId { get; private set; }
        public Vestuario? Vestuario { get; private set; }

        public List<Reserva> Reservas { get; private set; } = new List<Reserva>();

        public List<Servicio> Servicios { get; private set; } = new List<Servicio>();

        public decimal Recaudacion => Reservas.Where(r => !r.Cancelada).Sum(r => r.Importe);
        public int ReservasActivas => Reservas.Count(r => !r.Cancelada);

        private Cancha()
        {
        }

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

            Id = Guid.NewGuid();
            Deporte = deporte;
            TipoPiso = tipoPiso;
            JugadoresMax = jugadoresMax;
            PrecioPorHora = precioPorHora;
        }

        public Reserva Reservar(Cliente cliente, DateTime inicio, int horas)
        {
            ArgumentNullException.ThrowIfNull(cliente);

            if (horas <= 0)
                throw new ArgumentOutOfRangeException(nameof(horas), "La reserva debe durar al menos una hora.");

            if (!EstaLibre(inicio, horas))
                throw new InvalidOperationException("La cancha ya está reservada en ese horario.");

            var reserva = new Reserva(cliente, inicio, horas, PrecioPorHora * horas);
            Reservas.Add(reserva);
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

        public void AsignarVestuario(Vestuario vestuario)
        {
            Vestuario = vestuario;
            VestuarioId = vestuario.Id;
        }

        public void QuitarVestuario()
        {
            Vestuario = null;
            VestuarioId = null;
        }

        public void AgregarServicio(Servicio servicio)
        {
            if (Servicios.Any(s => s.Id == servicio.Id))
                throw new InvalidOperationException($"La cancha ya tiene el servicio {servicio.Nombre}.");

            Servicios.Add(servicio);
        }

        public void QuitarServicio(Guid servicioId)
        {
            var servicio = Servicios.FirstOrDefault(s => s.Id == servicioId)
                ?? throw new InvalidOperationException($"La cancha no tiene el servicio {servicioId}.");

            Servicios.Remove(servicio);
        }

        public bool EstaLibre(DateTime inicio, int horas)
        {
            var fin = inicio.AddHours(horas);
            return !Reservas.Any(r => !r.Cancelada && inicio < r.Fin && fin > r.Inicio);
        }

        public Reserva? ObtenerReserva(Guid idReserva) => Reservas.FirstOrDefault(r => r.Id == idReserva);

        public IReadOnlyList<Reserva> VerHistorial() => Reservas.AsReadOnly();
    }
}
