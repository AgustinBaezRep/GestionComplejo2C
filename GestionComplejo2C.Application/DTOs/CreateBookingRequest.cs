namespace GestionComplejo2C.Application.DTOs
{
    public record CreateBookingRequest(Guid CustomerId, DateTime Start, int Hours);
}
