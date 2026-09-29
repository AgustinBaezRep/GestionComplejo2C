using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IAutenticacionService
    {
        LoginResponse? Autenticar(LoginRequest request);
    }
}
