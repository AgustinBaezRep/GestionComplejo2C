namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IServicioToken
    {
        string GenerarToken(string usuario, string rol);
    }
}
