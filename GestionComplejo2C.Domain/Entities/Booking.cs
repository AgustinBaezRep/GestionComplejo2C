namespace GestionComplejo2C.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Guid CourtId { get; private set; }

        public Guid CustomerId { get; private set; }
        public Customer? Customer { get; private set; }

        public DateTime Start { get; private set; }
        public int Hours { get; private set; }
        public decimal Amount { get; private set; }
        public bool Cancelled { get; private set; }

        public DateTime End => Start.AddHours(Hours);

        private Booking()
        {
        }

        public Booking(Customer customer, DateTime start, int hours, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(customer);

            Id = Guid.NewGuid();
            Customer = customer;
            CustomerId = customer.Id;
            Start = start;
            Hours = hours;
            Amount = amount;
            Cancelled = false;
        }

        public void Cancel() => Cancelled = true;

        public override string ToString() =>
            $"{Start:dd/MM HH:mm}-{End:HH:mm} | {Customer?.Email ?? CustomerId.ToString()} | ${Amount}{(Cancelled ? " | CANCELLED" : "")}";
    }
}
