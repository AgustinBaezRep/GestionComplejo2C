namespace GestionComplejo2C.Domain.Entities
{
    public class Administrador : Usuario
    {
        private Administrador()
        {
        }

        public Administrador(string email, string passwordHash)
            : base(email, passwordHash)
        {
        }
    }
}
