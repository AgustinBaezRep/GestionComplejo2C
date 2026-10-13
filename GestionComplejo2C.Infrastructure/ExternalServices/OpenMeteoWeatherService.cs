using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using GestionComplejo2C.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace GestionComplejo2C.Infrastructure.ExternalServices
{
    public class OpenMeteoWeatherService : IWeatherService
    {
        private const int MaxForecastDays = 15;

        private readonly HttpClient httpClient;
        private readonly WeatherSettings settings;

        public OpenMeteoWeatherService(HttpClient httpClient, IOptions<WeatherSettings> settings)
        {
            this.httpClient = httpClient;
            this.settings = settings.Value;
        }

        public async Task<bool> IsRainingAsync(DateOnly date)
        {
            // Open-Meteo solo pronostica hasta 16 dias hacia adelante; mas alla no hay datos.
            if (date > DateOnly.FromDateTime(DateTime.Today).AddDays(MaxForecastDays))
            {
                return false;
            }

            var latitude = settings.Latitude.ToString(CultureInfo.InvariantCulture);
            var longitude = settings.Longitude.ToString(CultureInfo.InvariantCulture);
            var day = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            var url = $"v1/forecast?latitude={latitude}&longitude={longitude}&daily=precipitation_sum&timezone=auto&start_date={day}&end_date={day}";

            var response = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(url)
                ?? throw new HttpRequestException("La API del clima no devolvio datos.");

            var precipitation = response.Daily?.PrecipitationSum.FirstOrDefault() ?? 0;

            return precipitation > 0;
        }

        private class OpenMeteoResponse
        {
            [JsonPropertyName("daily")]
            public DailyData? Daily { get; set; }
        }

        private class DailyData
        {
            [JsonPropertyName("precipitation_sum")]
            public List<double?> PrecipitationSum { get; set; } = new List<double?>();
        }
    }
}
