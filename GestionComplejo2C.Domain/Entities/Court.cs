namespace GestionComplejo2C.Domain.Entities
{
    public class Court
    {
        public Guid Id { get; private set; }
        public string Sport { get; private set; } = string.Empty;
        public string SurfaceType { get; private set; } = string.Empty;
        public int MaxPlayers { get; private set; }
        public decimal PricePerHour { get; private set; }

        public Guid? LockerRoomId { get; private set; }
        public LockerRoom? LockerRoom { get; private set; }

        public List<Booking> Bookings { get; private set; } = new List<Booking>();

        public List<Amenity> Amenities { get; private set; } = new List<Amenity>();

        public decimal Revenue => Bookings.Where(b => !b.Cancelled).Sum(b => b.Amount);
        public int ActiveBookings => Bookings.Count(b => !b.Cancelled);

        private Court()
        {
        }

        public Court(string sport, string surfaceType, int maxPlayers, decimal pricePerHour)
        {
            if (string.IsNullOrWhiteSpace(sport))
                throw new ArgumentException("El deporte es obligatorio.", nameof(sport));

            if (string.IsNullOrWhiteSpace(surfaceType))
                throw new ArgumentException("El tipo de piso es obligatorio.", nameof(surfaceType));

            if (maxPlayers <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxPlayers), "Debe haber al menos un jugador.");

            if (pricePerHour <= 0)
                throw new ArgumentOutOfRangeException(nameof(pricePerHour), "El precio debe ser mayor a cero.");

            Id = Guid.NewGuid();
            Sport = sport;
            SurfaceType = surfaceType;
            MaxPlayers = maxPlayers;
            PricePerHour = pricePerHour;
        }

        public Booking Book(Customer customer, DateTime start, int hours)
        {
            ArgumentNullException.ThrowIfNull(customer);

            if (hours <= 0)
                throw new ArgumentOutOfRangeException(nameof(hours), "La reserva debe durar al menos una hora.");

            if (!IsAvailable(start, hours))
                throw new InvalidOperationException("La cancha ya está reservada en ese horario.");

            var booking = new Booking(customer, start, hours, PricePerHour * hours);
            Bookings.Add(booking);
            return booking;
        }

        public void CancelBooking(Guid bookingId)
        {
            var booking = GetBooking(bookingId)
                ?? throw new InvalidOperationException($"No existe la reserva {bookingId}.");

            if (booking.Cancelled)
                throw new InvalidOperationException("La reserva ya estaba cancelada.");

            booking.Cancel();
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice), "El precio debe ser mayor a cero.");

            PricePerHour = newPrice;
        }

        public void AssignLockerRoom(LockerRoom lockerRoom)
        {
            LockerRoom = lockerRoom;
            LockerRoomId = lockerRoom.Id;
        }

        public void RemoveLockerRoom()
        {
            LockerRoom = null;
            LockerRoomId = null;
        }

        public void AddAmenity(Amenity amenity)
        {
            if (Amenities.Any(a => a.Id == amenity.Id))
                throw new InvalidOperationException($"La cancha ya tiene el servicio {amenity.Name}.");

            Amenities.Add(amenity);
        }

        public void RemoveAmenity(Guid amenityId)
        {
            var amenity = Amenities.FirstOrDefault(a => a.Id == amenityId)
                ?? throw new InvalidOperationException($"La cancha no tiene el servicio {amenityId}.");

            Amenities.Remove(amenity);
        }

        public bool IsAvailable(DateTime start, int hours)
        {
            var end = start.AddHours(hours);
            return !Bookings.Any(b => !b.Cancelled && start < b.End && end > b.Start);
        }

        public Booking? GetBooking(Guid bookingId) => Bookings.FirstOrDefault(b => b.Id == bookingId);

        public IReadOnlyList<Booking> GetHistory() => Bookings.AsReadOnly();
    }
}
