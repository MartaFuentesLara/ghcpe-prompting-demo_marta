public class HotelBookingService : IHotelBooking
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HotelBookingService> _logger;

    public HotelBookingService(HttpClient httpClient, ILogger<HotelBookingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BookingInformation> BookHotelAsync(string userId, string hotelId, string roomId, DateTime beginDate, DateTime returnDate)
    {
        var bookingInformation = new BookingInformation
        {
            Id = Guid.NewGuid().ToString(),
            HotelId = hotelId,
            RoomId = roomId,
            BeginDate = beginDate,
            ReturnDate = returnDate,
            UserId = userId
        };

        return bookingInformation;
    }

    public async Task<IEnumerable<HotelInformation>> GetHotelsAsync(DateTime date)
    {
        var hotels = new List<HotelInformation>
        {
            new HotelInformation
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Hotel A",
                Location = "Location A",
                Price = 100
            },
            new HotelInformation
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Hotel B",
                Location = "Location B",
                Price = 200
            }
        };

        return hotels;
    }
}