namespace GestionComplejo2C.Infrastructure.ExternalServices
{
    public class JwtSettings
    {
        public const string SeccionConfiguracion = "Jwt";

        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int MinutosDeExpiracion { get; set; } = 60;
    }
}
