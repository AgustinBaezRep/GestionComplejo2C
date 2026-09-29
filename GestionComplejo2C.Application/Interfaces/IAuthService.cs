using GestionComplejo2C.Application.DTOs;

namespace GestionComplejo2C.Application.Interfaces
{
    public interface IAuthService
    {
        LoginResponse? Authenticate(LoginRequest request);
    }
}
