using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Infrastructure.ExternalServices
{
    public class ServicioHashPasswordBCrypt : IServicioHashPassword
    {
        public string Hashear(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verificar(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
