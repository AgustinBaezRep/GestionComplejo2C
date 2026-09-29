using System.Security.Claims;
using System.Text;
using GestionComplejo2C.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace GestionComplejo2C.Infrastructure.ExternalServices
{
    public class ServicioTokenJwt : IServicioToken
    {
        private readonly JwtSettings settings;

        public ServicioTokenJwt(IOptions<JwtSettings> settings)
        {
            this.settings = settings.Value;
        }

        public string GenerarToken(string usuario, string rol)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(settings.MinutosDeExpiracion),
                SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, usuario),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, usuario),
                    new Claim(ClaimTypes.Role, rol)
                })
            };

            return new JsonWebTokenHandler().CreateToken(descriptor);
        }
    }
}
