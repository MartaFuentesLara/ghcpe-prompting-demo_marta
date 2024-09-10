namespace GhcpDemo.Api.Interfaces;


public interface ICarRental
{
    public Task<IEnumerable<CarInformation>> GetCarsAsync(string location, DateTime date);

    public Task<CarRentalInformation> RentCarAsync(string userId, string carId, DateTime beginDate, DateTime returnDate);
};