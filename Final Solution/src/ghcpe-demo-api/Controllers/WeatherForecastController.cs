using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

namespace GhcpDemo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IHttpClientFactory clientFactory, ILogger<WeatherForecastController> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get weather forecast data from the Open Meteo API and return it as an instance of OpenMeteoResponse
    /// </summary>
    /// <returns></returns>
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<OpenMeteoResponse> Get(double latitude, double longitude)
    {
        var client = _clientFactory.CreateClient("Open-meteo");
        var url = $"forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m";

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var weatherData = JsonConvert.DeserializeObject<OpenMeteoResponse>(responseContent);
            return weatherData ?? new OpenMeteoResponse();
        }
        else
        {
            _logger.LogError($"Error fetching weather data: {response.StatusCode}");
            return new OpenMeteoResponse();
        }
    }
}
