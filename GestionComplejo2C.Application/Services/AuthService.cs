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

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            // Verify y GenerateToken quedan sincronicos: son trabajo de CPU, no esperan ninguna E/S.
            if (user == null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            return new LoginResponse(tokenService.GenerateToken(user.Email, user.GetType().Name));
        }
    }
}
