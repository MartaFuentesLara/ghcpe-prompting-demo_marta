using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Newtonsoft.Json;

namespace GhcpDemo.Api.Tests.Controllers;


public class WeatherForecastControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherForecastControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData(40.4165, -3.70256)] // Madrid
    [InlineData(36.7213, -4.4214)] // Málaga
    [InlineData(41.3851, 2.1734)] // Barcelona
    [InlineData(37.3891, -5.9845)] // Sevilla
    [InlineData(43.3623, -8.4115)] // Coruña
    public async Task Get_ReturnsWeatherForecasts_PropertlyDeserialized(double latitude, double longitude)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/WeatherForecast?latitude={latitude}&longitude={longitude}");

        // Assert
        response.Should().NotBeNull();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        // Assert content type is JSON using fluent assertions
        response?.Content.Headers.ContentType?.MediaType
            .Should().Be("application/json");

        var responseContent = await response?.Content.ReadAsStringAsync();
        var weatherData = JsonConvert.DeserializeObject<OpenMeteoResponse>(responseContent);
        weatherData.Should().NotBeNull();
        weatherData?.Current.Should().NotBeNull();
        weatherData?.Hourly.Should().NotBeNull();
    }
}
