using GestionComplejo2C.Application.DTOs;
using GestionComplejo2C.Application.Interfaces;
using GestionComplejo2C.Domain.Entities;
using GestionComplejo2C.Domain.Interfaces;

namespace GestionComplejo2C.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly ICourtRepository courtRepository;
        private readonly IUserRepository userRepository;
        private readonly IWeatherService weatherService;

        public BookingService(ICourtRepository courtRepository, IUserRepository userRepository, IWeatherService weatherService)
        {
            this.courtRepository = courtRepository;
            this.userRepository = userRepository;
            this.weatherService = weatherService;
        }

        public async Task<BookingResponse> CreateAsync(Guid courtId, CreateBookingRequest request, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            var customer = await userRepository.GetCustomerByIdAsync(request.CustomerId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe un cliente con el id {request.CustomerId}.");

            var isRaining = await weatherService.IsRainingAsync(DateOnly.FromDateTime(request.Start), cancellationToken);

            var booking = court.Book(customer, request.Start, request.Hours, isRaining);

            courtRepository.AddBooking(booking);
            await courtRepository.SaveChangesAsync(cancellationToken);

            return BookingResponse.From(booking);
        }

        public async Task<IReadOnlyList<BookingResponse>> GetAllAsync(Guid courtId, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            return court.GetHistory().Select(BookingResponse.From).ToList();
        }

        public async Task<BookingResponse?> GetByIdAsync(Guid courtId, Guid id, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            var booking = court.GetBooking(id);

            return booking == null ? null : BookingResponse.From(booking);
        }

        public async Task<bool> CancelAsync(Guid courtId, Guid id, CancellationToken cancellationToken = default)
        {
            var court = await GetCourtAsync(courtId, cancellationToken);

            if (court.GetBooking(id) == null)
            {
                return false;
            }

            court.CancelBooking(id);

            await courtRepository.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task<Court> GetCourtAsync(Guid courtId, CancellationToken cancellationToken) =>
            await courtRepository.GetByIdAsync(courtId, cancellationToken)
                ?? throw new KeyNotFoundException($"No existe una cancha con el id {courtId}.");
    }
}
