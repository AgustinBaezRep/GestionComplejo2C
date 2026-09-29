namespace GestionComplejo2C.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string username, string role);
    }
}
