using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly IRepositorioUsuarios repositorioUsuarios;
        private readonly IServicioToken servicioToken;
        private readonly IServicioHashPassword servicioHashPassword;

        public AutenticacionService(
            IRepositorioUsuarios repositorioUsuarios,
            IServicioToken servicioToken,
            IServicioHashPassword servicioHashPassword)
        {
            this.repositorioUsuarios = repositorioUsuarios;
            this.servicioToken = servicioToken;
            this.servicioHashPassword = servicioHashPassword;
        }

        public LoginResponse? Autenticar(LoginRequest request)
        {
            var usuario = repositorioUsuarios.ObtenerPorEmail(request.Email);

            if (usuario == null || !servicioHashPassword.Verificar(request.Password, usuario.PasswordHash))
            {
                return null;
            }

            return new LoginResponse(servicioToken.GenerarToken(usuario.Email, usuario.GetType().Name));
        }
    }
}
