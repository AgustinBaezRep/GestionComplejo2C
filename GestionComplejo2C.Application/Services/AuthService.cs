using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenService tokenService;
        private readonly IPasswordHasher passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.tokenService = tokenService;
            this.passwordHasher = passwordHasher;
        }

        public LoginResponse? Authenticate(LoginRequest request)
        {
            var user = userRepository.GetByEmail(request.Email);

            if (user == null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return new LoginResponse(tokenService.GenerateToken(user.Email, user.GetType().Name));
        }
    }
}
