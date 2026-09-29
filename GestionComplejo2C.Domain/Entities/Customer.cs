namespace GestionComplejo2C.Domain.Entities
{
    public class Customer : User
    {
        public List<Booking> Bookings { get; private set; } = new List<Booking>();

        private Customer()
        {
        }

        public Customer(string email, string passwordHash)
            : base(email, passwordHash)
        {
        }
    }
}
