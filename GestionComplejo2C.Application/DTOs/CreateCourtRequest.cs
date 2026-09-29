namespace GestionComplejo2C.Application.DTOs
{
    public record CreateCourtRequest(string Sport, string SurfaceType, int MaxPlayers, decimal PricePerHour);
}
