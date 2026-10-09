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

        public async Task<BookingResponse> CreateAsync(Guid courtId, CreateBookingRequest request)
        {
            var court = GetCourt(courtId);

            var customer = userRepository.GetCustomerById(request.CustomerId)
                ?? throw new KeyNotFoundException($"No existe un cliente con el id {request.CustomerId}.");

            var isRaining = await weatherService.IsRainingAsync(DateOnly.FromDateTime(request.Start));

            var booking = court.Book(customer, request.Start, request.Hours, isRaining);

            courtRepository.AddBooking(booking);
            courtRepository.SaveChanges();

            return BookingResponse.From(booking);
        }

        public IReadOnlyList<BookingResponse> GetAll(Guid courtId) =>
            GetCourt(courtId).GetHistory().Select(BookingResponse.From).ToList();

        public BookingResponse? GetById(Guid courtId, Guid id)
        {
            var booking = GetCourt(courtId).GetBooking(id);

            return booking == null ? null : BookingResponse.From(booking);
        }

        public bool Cancel(Guid courtId, Guid id)
        {
            var court = GetCourt(courtId);

            if (court.GetBooking(id) == null)
            {
                return false;
            }

            court.CancelBooking(id);

            courtRepository.SaveChanges();

            return true;
        }

        private Court GetCourt(Guid courtId) =>
            courtRepository.GetById(courtId)
                ?? throw new KeyNotFoundException($"No existe una cancha con el id {courtId}.");
    }
}
