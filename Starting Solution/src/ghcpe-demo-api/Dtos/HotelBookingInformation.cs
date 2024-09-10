public class HotelInformation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
}

public class BookingInformation
{
    public string Id { get; set; }
    public string HotelId { get; set; }
    public string RoomId { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public string UserId { get; set; }
}