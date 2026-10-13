namespace GestionComplejo2C.Domain.Interfaces
{
    public interface IWeatherService
    {
        Task<bool> IsRainingAsync(DateOnly date);
    }
}
