using GestionComplejo2C.Domain.Entities;

namespace GestionComplejo2C.Application.DTOs
{
    public record BookingResponse(
        Guid Id,
        Guid CourtId,
        Guid CustomerId,
        string? CustomerEmail,
        DateTime Start,
        DateTime End,
        int Hours,
        decimal Amount,
        bool Cancelled)
    {
        public static BookingResponse From(Booking booking) => new(
            booking.Id,
            booking.CourtId,
            booking.CustomerId,
            booking.Customer?.Email,
            booking.Start,
            booking.End,
            booking.Hours,
            booking.Amount,
            booking.Cancelled);
    }
}
