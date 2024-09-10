using System.Net;
using System.Text;
using FluentAssertions;
using GhcpDemo.Api.Controllers;
using GhcpDemo.Api.Dtos;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace GhcpDemo.Api.Tests.Controllers
{
    public class WeatherForecastControllerTests
    {
        // First shot with the unit test it worked until we start the refactoring with open-meteo
        // [Fact]
        // public void Get_ReturnsWeatherForecasts()
        // {
        //     // Arrange
        //     var controller = new WeatherForecastController();

        //     // Act
        //     var result = controller.Get();

        //     // Assert
        //     result.Should().NotBeNull().And.BeAssignableTo<IEnumerable<WeatherForecast>>();

        //     var weatherForecasts = result.ToList();
        //     weatherForecasts.Should().HaveCount(5);

        //     var expectedDate = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        //     foreach (var forecast in weatherForecasts)
        //     {
        //         forecast.Date.Should().Be(expectedDate);
        //         expectedDate = expectedDate.AddDays(1);

        //         forecast.TemperatureC.Should().BeInRange(-20, 55);
        //         forecast.Summary.Should().BeOneOf(Constants.Summaries);
        //     }
        // }

        // Second shot with the unit test after the open-meteo refactoring. 
        /// This code was initially suggested by GHCPE, have a look at the line number 21 and the declaration of the test.
        // [Fact]
        // public void Get_ReturnsWeatherForecasts()
        // {
        //     // Arrange
        //     var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        //     var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        //     var controller = new WeatherForecastController(httpClientFactoryMock.Object, loggerMock.Object);

        //     // Act
        //     var result = controller.Get();

        //     // Assert
        //     result.Should().NotBeNull().And.BeAssignableTo<IEnumerable<WeatherForecast>>();

        //     var weatherForecasts = await result;
        //     weatherForecasts.Should().HaveCount(5);

        //     var expectedDate = DateTime.Now.Date.AddDays(1);
        //     foreach (var forecast in weatherForecasts)
        //     {
        //         forecast.Date.Should().Be(expectedDate.Date);
        //         expectedDate = expectedDate.AddDays(1);

        //         forecast.TemperatureC.Should().BeInRange(-20, 55);
        //         forecast.Summary.Should().BeOneOf(Constants.Summaries);
        //     }
        // }

        /// This code was suggested by Copilot after the next prompt:
        /// /fix Hi, Copilot. The selected code is not going to work as expected. The Get method under test is async and returns a Task. 
        /// Could you please fix the test to wait on controller.Get(), please?
        /// Obviously, it wont work we'll take a different approach
        // [Fact]
        // public async Task Get_ReturnsWeatherForecasts()
        // {
        //     // Arrange
        //     var loggerMock = new Mock<ILogger<WeatherForecastController>>();
        //     var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        //     var controller = new WeatherForecastController(httpClientFactoryMock.Object, loggerMock.Object);

        //     // Act
        //     var result = await controller.Get();

        //     // Assert
        //     result.Should().NotBeNull().And.BeAssignableTo<IEnumerable<WeatherForecast>>();
        //     result.Should().HaveCount(5);

        //     var expectedDate = DateTime.Now.Date.AddDays(1);
        //     foreach (var forecast in result)
        //     {
        //         forecast.Date.Should().Be(DateOnly.FromDateTime(expectedDate));
        //         expectedDate = expectedDate.AddDays(1);

        //         forecast.TemperatureC.Should().BeInRange(-20, 55);
        //         forecast.Summary.Should().BeOneOf(Constants.Summaries);
        //     }
        // }

        private readonly Mock<ILogger<WeatherForecastController>> _loggerMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

        public WeatherForecastControllerTests()
        {
            _loggerMock = new Mock<ILogger<WeatherForecastController>>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public async Task Get_ReturnsWeatherForecasts_PropertlyDeserialized()
        {
            //const string mockedResponse = "{'current':{'time': '2022 - 01 - 01T15: 00','temperature_2m': 2.4,'wind_speed_10m': 11.9,},'hourly':{'time': ['2022 - 07 - 01T00: 00','2022 - 07 - 01T01: 00'],'wind_speed_10m': [3.16,3.02],'temperature_2m': [13.7,13.3],'relative_humidity_2m':[82,83]}}";
            const string mockedResponse = @"{
              ""current"": {
                ""time"": ""2022-01-01T15:00"",
                ""temperature_2m"": 2.4,
                ""wind_speed_10m"": 11.9,
              },
              ""hourly"": {
                ""time"": [""2022-07-01T00:00"",""2022-07-01T01:00""],
                ""wind_speed_10m"": [3.16,3.02],
                ""temperature_2m"": [13.7,13.3],
                ""relative_humidity_2m"": [82,83],
              }
            }";


            // Arrange
            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(mockedResponse, Encoding.UTF8, "application/json")
                });

            _httpClientFactoryMock.Setup(x => x.CreateClient("Open-meteo")).Returns(new HttpClient(httpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://api.open-meteo.com/v1/")
            });

            var controller = new WeatherForecastController(_httpClientFactoryMock.Object, _loggerMock.Object);

            // Act
            var madridLatitude = 40.4165;
            var madridLongitude = -3.70256;

            var result = await controller.Get(madridLatitude, madridLongitude);

            // Assert
            // Add your assertions here
            result.Should().NotBeNull().And.BeAssignableTo<OpenMeteoResponse>();

            // We know the Get endpoint calls once the open-meteo api, lets create an assert on this.
            httpMessageHandler
                .Protected()
                .Verify("SendAsync", Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                        ItExpr.IsAny<CancellationToken>());

            // Let's assert on the content serialized. 
            // Current and hourly should not be null.
            result.Current.Should().NotBeNull();
            result.Hourly.Should().NotBeNull();

            // Current time should be 2022-01-01T15:00
            result.Current?.Time.Should().Be(DateTime.Parse("2022-01-01T15:00"));
            // Current temperature_2m should be 2.4
            result.Current?.Temperature_2m.Should().Be(2.4);
            // Current wind_speed_10m should be 11.9
            result.Current?.Wind_Speed_10m.Should().Be(11.9);

            // Hourly time should be ["2022-07-01T00:00","2022-07-01T01:00"]
            result.Hourly?.Time.Should().BeEquivalentTo(new List<DateTime>
            {
                DateTime.Parse("2022-07-01T00:00"),
                DateTime.Parse("2022-07-01T01:00")
            });
            // Hourly wind_speed_10m should be [3.16,3.02]
            result.Hourly?.Wind_Speed_10m.Should().BeEquivalentTo(new List<double> { 3.16, 3.02 });
            // Hourly temperature_2m should be [13.7,13.3]
            result.Hourly?.Temperature_2m.Should().BeEquivalentTo(new List<double> { 13.7, 13.3 });
            // Hourly relative_humidity_2m should be [82,83]
            result.Hourly?.Relative_Humidity_2m.Should().BeEquivalentTo(new List<int> { 82, 83 });
        }
    }
}
