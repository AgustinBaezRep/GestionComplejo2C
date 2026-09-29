namespace GestionComplejo2C.Domain.Entities
{
    public class Administrator : User
    {
        private Administrator()
        {
        }

        public Administrator(string email, string passwordHash)
            : base(email, passwordHash)
        {
        }
    }
}
