public class HotelInformation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class BookingInformation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string HotelId { get; set; } = Guid.NewGuid().ToString();
    public string RoomId { get; set; } = Guid.NewGuid().ToString();
    public DateTime? BeginDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? UserId { get; set; }
}