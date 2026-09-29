namespace GestionComplejo2C.Domain.Entities
{
    public class Cliente : Usuario
    {
        public List<Reserva> Reservas { get; private set; } = new List<Reserva>();

        private Cliente()
        {
        }

        public Cliente(string email, string passwordHash)
            : base(email, passwordHash)
        {
        }
    }
}
