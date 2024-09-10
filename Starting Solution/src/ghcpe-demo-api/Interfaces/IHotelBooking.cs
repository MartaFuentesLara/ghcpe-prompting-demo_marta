namespace GhcpDemo.Api.Interfaces;

public interface IHotelBooking
{
    public Task<IEnumerable<HotelInformation>> GetHotelsAsync(DateTime date);

    public Task<BookingInformation> BookHotelAsync(string userId, string hotelId, string roomId, DateTime beginDate, DateTime returnDate);
}