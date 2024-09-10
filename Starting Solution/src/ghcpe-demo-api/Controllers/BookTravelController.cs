using Microsoft.AspNetCore.Mvc;
using GhcpDemo.Api.Interfaces;

namespace GhcpDemo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BookTravelController: ControllerBase {

    private readonly IHotelBooking _hotelBooking;
    private readonly ICarRental _carRental;
    private readonly ILogger<BookTravelController> _logger;

    public BookTravelController(ILogger<BookTravelController> logger, IHotelBooking hotelBooking, ICarRental carRental) {
        _logger = logger;
        _hotelBooking = hotelBooking;
        _carRental = carRental;
    }
}