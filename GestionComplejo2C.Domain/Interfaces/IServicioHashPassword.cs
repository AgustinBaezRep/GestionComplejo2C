namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IServicioHashPassword
    {
        string Hashear(string password);

        bool Verificar(string password, string passwordHash);
    }
}
