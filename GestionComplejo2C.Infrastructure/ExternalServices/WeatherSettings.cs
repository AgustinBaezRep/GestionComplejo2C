namespace GestionComplejo2C.Infrastructure.ExternalServices
{
    public class WeatherSettings
    {
        public const string SectionName = "Weather";

        public string BaseUrl { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
