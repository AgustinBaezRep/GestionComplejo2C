namespace GestionComplejo2C.Domain.Entities
{
    public abstract class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;

        protected User()
        {
        }

        protected User(string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es obligatorio.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("El hash del password es obligatorio.", nameof(passwordHash));

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
