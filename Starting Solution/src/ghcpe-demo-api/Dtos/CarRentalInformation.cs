
public class CarRentalInformation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string CarId { get; set; } = Guid.NewGuid().ToString();
    public DateTime? BeginDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? UserId { get; set; }
}

public class CarInformation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Model { get; set; } = string.Empty;
}