# copilot-webcast
Repository with the code demoed during the webcast

## Description
This demo is a classic web api with a single controller and several endpoints. 
The controller is a WeatherForecast simulator that returns a list of weather forecasts for the next 5 days.

## Purpose of the demo
The purpose of the demo is to use GitHub Copilot Enterprise to refactor and improve the code throug several steps.

## A few notes before starting
You will see, during the demo, we are proposing some prompts to Copilot. In the Complete Solution project, what you will find is the output GitHub Copilot provided to us as results.
Please, note that **any Generative AI is not deterministic**, so the results may vary from one execution to another.
This means that, even if you use the same prompt, you may get different results. Use this in your advantage: experiment, try different prompts, and see what you get.

## Steps
### Setup Ask Copilot shortcut in Visual Studio
Go to Tools -> Options -> Environment -> Keyboard and search for "Ask.Copilot". Assign a shortcut to it: i.e. Ctrl+A, Ctrl+C

### 1. Understand the code and document a little (5')
1. **Initial code**: The initial code is a classic web api with a single controller and several endpoints. The controller is a WeatherForecast simulator that returns a list of weather forecasts for the next 5 days.
1. **Understand the code**: Ask Copilot to explain the `Get` endpoint.
	1. **Open the WeatherForecastController.cs file**: Open the file and select the `Get` endpoint.
	1. **Use the following prompt**: _/explain_
	1. **Copy the explanation**: Select and copy the explanation provided by Copilot.
	1. **Open project's README.md**: _Please, using the #file:'README.md" and the markdown syntax include this explanation : \<explanation from previous step\>_
1. **Document Get endpoint**: Select the `Get` endpoint and use Copilot to generate the XML documentation.
	1. **Use the following prompt**: _/doc_ 

### 2. Create a few unit tests (6')
1. **Use the following prompt**: _Hi, Copilot. Let's start over, shall we?_
1. **Create a unit test**: Ask Copilot to create a unit test for the `Get` endpoint.
	1. **Use the following prompt**: _/tests Hello, Copilot. I need to create a xUnit unit test for the 'GET' endpoint in the #file:'WeatherForecast.cs' . Could you help me, please?_
	1. **Copy the test**: Use the interface to copy the test to the clipbard.
	1. **Open the test file**: Open the file `WeatherForecastControllerTests.cs` and paste the test in it.
1. **Review the code provided by Copilot**: Review the test, it may not compile. If it's the case, make adjustments in the code or ask Copilot to refactor the Test.
    1. In our test, Copilot tried to access the private property **Summaries**, we decided to refactor the code and extract it to a **public static constant**.
1. **Compile and run the test**: Validate that the tests are running and passing.
1. **Document the test**: Use Copilot to generate the XML documentation for the test.
	1. **Select the test**: Select the test and use the prompt _/doc_ to generate the XML documentation.
1. **Refactor a little the test**: Ask Copilot to use FluentAssertions instead of the classic xUnit assertions.
	1. **Use the following prompt**: _Could you please, use Fluent Assertions instead of XUnit Assert?_
1. **Compile and run the test**: Validate that the tests are running and passing.

### 3. Refactor the code to use open-meteo API (10')
Let's start thinking about what do we need to consume a real weather API. We need to:
1. **Start a new chat**: In example, you may use the _'Delete thread'_ or _'Create new thread'_ icon in the IDE.
1. **Organize a little the ideas with the help of GHCPE. Use this prompt**: _Hi, Copilot. I want to create a mermaid diagram, that shows next. 
	1 Receive an instance of IHttpClientFactory in the WeatherForecastController and store in a class property. 
	2 When the user calls the 'Get' endpoint we use the factory to get a HttpClient instance.
    3 Use the client to call open-meteo API and get the weather forecast.
    4 Map the returned json to a response.
    5 Return the response to the client. Can you help me with this?_
1. **Check the response**: You can check the response provided using https://mermaid.live/ or any other mermaid diagram renderer.
1. **Refactor the code**: Ask Copilot to refactor the code to implement the previous diagram.
	1. **Use the following prompt**: _/fix  With the given information, could you change #file:'WeatherForecastController.cs' so, instead of the current implementation it receives an IHttpClientFactory in the constructor, and calls open-meteo api in the get endpoint?_   
    **Note**: GitHub Copilot returned the a nice piece of code at the first attempt, you can check at the end of this section. It won't work as expected, but is a very good starting point. Your results may differ.
    1. **Document the endpoint**: Start typing _///_ and let Copilot complete the header (again you'll see the suggestion at the end of this section)
	

    1. **Open Program.cs**: We need to prepare the injection of the IHttpClientFactory
    1. **Go to line 10 and write the next comment**: _// Let's create prepare the HttpClientFactory so we can use it to make requests to the open-meteo API_
    1. **Press <Enter> and wait for a Copilot suggestion**: In our case the code proposed was pretty accurate, but needed to add additional information.
    1. **Select the code suggested and write the following propmt**: _/fix Please, create a named client called "Open-meteo" instead_ **Note**: You may prefer to do it on your own as this is a quite straight-forward change. The important thing here, is that Copilot already suggested the **almost** proper open-meteo URL with the right version (it suggested http://api.open-meteo.com/v1/, but the right one works over HTTPS)
    
```cs
using System.Net.Http;
using Newtonsoft.Json;
using GhcpDemo.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GhcpDemo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IHttpClientFactory clientFactory, ILogger<WeatherForecastController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Get weather forecast data from the Open Meteo API and return it as a list of WeatherForecast objects
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://api.open-meteo.com/v1/forecast");
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var weatherData = JsonConvert.DeserializeObject<OpenMeteoResponse>(responseContent);
                return weatherData ?? new OpenMeteoResponse();
            }
            else
            {
                _logger.LogError($"Error fetching weather data: {response.StatusCode}");
                return Array.Empty<WeatherForecast>();
            }
        }
    }
}
```

```cs
// Let's create prepare the HttpClientFactory so we can use it to make requests to the open-meteo API
builder.Services.AddHttpClient("Open-meteo", conf =>
{
    conf.BaseAddress = new Uri("http://api.open-meteo.com/v1/");
    conf.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

```


Notes: To be tested and refined. --> Add Intermediate  solution?

### 4. Improve and refactor the code with the aid of testing (+14?')
Now we should start validating our code. Using tests could be a good option for doing that.
1. **Go to your WeatherForecastControllerTests.cs**: Either using Copilot Chat or using suggestion use the next prompt: _/fix The WeatherForecastController constructor now requires an instance of ILogger and IHttpClientFactory, could you please refactor the code to include mocks of both?_
2. **Review the code proposed**: In our case the code was quite buggy. It forgot that Get endpoint is returning a Task, so we needed to ask refactor to fix the code a little here and there.
3. **Used the next prompt**: _/fix Hi, Copilot. The suggested code is not going to work as expected. The Get method under test is async and returns a Task. Could you please fix the test to wait on controller.Get(), please?_
**Note**: Copilot provided this answer (useful to mention, but it will depend on your current experience)
4. **Let's change the approach**: Seems like GHCPE is confused with our previous "fake weather method". It is not really clear what we want to achieve, so let's try a TDD approach instead.
5. **Create a DTO to hold open-meteo respone**: Go to [open-meteo](https://open-meteo.com/) and have a look at the simple example shown at the begining of the web. We'll use it for our code.
```bash
$ curl "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m"
```
```json
{
  "current": {
    "time": "2022-01-01T15:00"
    "temperature_2m": 2.4,
    "wind_speed_10m": 11.9,
  },
  "hourly": {
    "time": ["2022-07-01T00:00","2022-07-01T01:00", ...]
    "wind_speed_10m": [3.16,3.02,3.3,3.14,3.2,2.95, ...],
    "temperature_2m": [13.7,13.3,12.8,12.3,11.8, ...],
    "relative_humidity_2m": [82,83,86,85,88,88,84,76, ...],
  }
}
```
6. **Create a new file**: Create a new file called OpenMeteoResponse.cs under /Dto folder
7. **Let's create a comment to give GHCPE some context**: On the top of the new file write the next comment.
```csharp
//{
//    "current": {
//        "time": "2022-01-01T15:00"
//        "temperature_2m": 2.4,
//        "wind_speed_10m": 11.9,
//    },
//    "hourly": {
//        "time": ["2022-07-01T00:00","2022-07-01T01:00", ...]
//        "wind_speed_10m": [3.16,3.02,3.3,3.14,3.2,2.95, ...],
//        "temperature_2m": [13.7,13.3,12.8,12.3,11.8, ...],
//        "relative_humidity_2m": [82,83,86,85,88,88,84,76, ...],
//    }
//}
```
8. **Start writing some code and wait for GHCE to complete it for you**: Just type _public record OpenMeteoResponse_ and wait a few seconds until copilot gives you some responses.
9. **Modify the controller to adjust a little the Get method**: Go to WeatherForecastController.cs, select the full method and, asking Copilot, write down this prompt. _/fix Hi, Copilot. I need to fix this method so it can call open meteo using this pattern "https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m". Please, ensure that latitude, and longitude are params passed by query string to the get endpoint and the results are returned using OpenMeteoResponse_. Copilot provided the next code (we can improve it, definetely, i.e. as we are using the named client we can ask Copilot to make the proper change):
```csharp
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<OpenMeteoResponse> Get(double latitude, double longitude)
    {
        var client = _clientFactory.CreateClient("Open-meteo");
        var url = $"forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        HttpResponseMessage response = await client.SendAsync(request);

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
```
11. **Run this prompt**: _@workspace /fix  Hi, Copilot. As we have defined in #file:Program.cs a named HttpClient called "Open-meteo", please change the selected get endpoint in #selection to use it instead, ensure the url is changed properly, please_ (this prompt was run using Visual Studio Code, ensure it works with Visual Studio too)
**Note**: This prompt has been created using Visual Studio Code, it may not work in Visual Studio or any other IDE.
12. **Let's try again with the unit test**: Using the same chat - this way we keep the context - we use the next prompt: _@workspace /tests Please, create a xUnit unit tests for the get endpoint in file #file:WeatherForecastController.cs . Please, mock the IHttpClientFactory and the ILogger_
**Note**: This prompt has been created using Visual Studio Code, it may not work in Visual Studio or any other IDE.
```csharp
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ghcpe_demo_api_tests.Controllers
{
    public class WeatherForecastControllerTests
    {
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
            // Arrange
            var controller = new WeatherForecastController(_httpClientFactoryMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Get();

            // Assert
            result.Should().NotBeNull().And.BeAssignableTo<IEnumerable<WeatherForecast>>();
            result.Should().HaveCount(5);

            var expectedDate = DateTime.Now.Date.AddDays(1);
            foreach (var forecast in result)
            {
                forecast.Date.Should().Be(DateOnly.FromDateTime(expectedDate));
                expectedDate = expectedDate.AddDays(1);

                forecast.TemperatureC.Should().BeInRange(-20, 55);
                forecast.Summary.Should().BeOneOf(Constants.Summaries);
            }
        }
    }
}
```
12. **Let's setup the IHttpClientFactory behaviour**: _Please, customize the mock behaviour for IHttpClientFactory, so when CreateClient is called with "Open-meteo" is called, it returns a HttpClient mock_
**Note**: This is a very good example of how you can learn with Copilot (HttpClient has not a public contructor, have a look at what Copilot says)
_**Mocking HttpClient directly is not recommended because HttpClient does not have a public constructor that takes a HttpMessageHandler, and its methods are not virtual, which makes it difficult to mock with Moq. However, you can use HttpMessageHandler to simulate the behavior of HttpClient. Here's how you can do it:**_
```csharp
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;

namespace ghcpe_demo_api_tests.Controllers
{
    public class WeatherForecastControllerTests
    {
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
        }
    }
}
```
So we keep this answer, and work a little on it ourselves, to see what we can learn. (We are going to use this json as part of our mocked response)
```json
{'current':{'time': '2022 - 01 - 01T15: 00','temperature_2m': 2.4,'wind_speed_10m': 11.9,},'hourly':{'time': ['2022 - 07 - 01T00: 00','2022 - 07 - 01T01: 00'],'wind_speed_10m': [3.16,3.02],'temperature_2m': [13.7,13.3],'relative_humidity_2m':[82,83]}}
```
13. **Let's make a call for a known city**: In the WeahterForecastControllerTests.cs, you'll see that the 'Act' part of the test, calls Get with a 0, 0 latitude and longitude. Before making the call, start writting _var madridLatitude_ and let Copilot complete the value, do the same for _var madridLongitude_ (You may choose any city you like).
14. **Let's refactor a little the Unit Test code**: We know the Get endpoint runs an HTTP Get request, so
we know that the 'Mock<HttpResponseMessageHandler>.Setup' should consider only Get requests.
15. **Use the next prompt**: _/fix Hi, Copilot, please change the setup for the HttpRequestMessage, so it does not admit Any request, and only Get requests_
16. **Enhance the Assert part**: We know the Get endpoint calls once the open-meteo api, lets create an assert on this.
17. **Write the next comment at the end of your test**: _// We know the Get endpoint calls once the open-meteo api, lets create an assert on this._Press <enter> and wait a few seconds for copilot to provide a suggestion.
```csharp
httpMessageHandler.Protected().Verify(
	"SendAsync",
	Times.Once(),
	ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
	ItExpr.IsAny<CancellationToken>()
);
```
18. **Write the next comment at the end of your test**: _// Current and hourly should not be null._ Wait a few seconds for Copilot to suggest.
19. **Write the next comment at the end of your test**: _// Current time should be 2022-01-01T15:00._ Wait a few seconds for Copilot to suggest.
20. **Press <Enter> and wait for Copilot to suggest**: In our case, Copilot suggested som assertions on the values for current and hourly without manual intervention.
```csharp
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
```

## 5. Integration tests with GitHub Copilot (?')
Now we have a Unit Test completed - we could refactor a little the code, but we are going to leave it as is - we can start thinking about an integration test.
1. **Start creating a new file for the integration test**: Create a new file called WeatherForecastControllerIntegrationTests.cs
2. **Delete the empty class**: We may also remove all the using sentences.
3. **Open Copilot chat and write the next prompt**: First ensure that you remove any previous active chat: _/tests We need to create an integration test for the get endpoint defined in the controller #file:'WeatherForecastController.cs' . Could you, please, help me with it?_
4. **Analyze the response provided**: In our case, it provided an Unit test instead of an integration tests, so we needed to refine the context: _/tests , Thanks, Copilot, but this is not an integration test, this is a unit test. We want to test the controller allowing it to really call open-meteo API and validating everything works as expected._
5. **In our second shot we got something more as expected**:
```csharp
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GhcpDemo.Api.Tests.Controllers
{
    public class WeatherForecastControllerIntegrationTests : IClassFixture<WebApplicationFactory<YourWebApplicationNamespace.Startup>>
    {
        private readonly WebApplicationFactory<YourWebApplicationNamespace.Startup> _factory;

        public WeatherForecastControllerIntegrationTests(WebApplicationFactory<YourWebApplicationNamespace.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsExpectedWeatherForecast()
        {
            // Arrange
            var client = _factory.CreateClient();
            var latitude = 40.4165;
            var longitude = -3.70256;

            // Act
            var response = await client.GetAsync($"/WeatherForecast?latitude={latitude}&longitude={longitude}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", 
                         response.Content.Headers.ContentType.ToString());
        }
    }
}
```
**Note**: There is some refactoring needed. Copilot propose to use Startup, but we need to use Program instead
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;

namespace GhcpDemo.Api.Tests.Controllers;


public class WeatherForecastControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WeatherForecastControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ReturnsStatusCode200AndJsonContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var latitude = 40.4165;
        var longitude = -3.70256;

        // Act
        var response = await client.GetAsync($"/WeatherForecast?latitude={latitude}&longitude={longitude}");

        // Assert response and ContentType are not null using Fluent Assertions
        response.Should().NotBeNull();
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        // Assert content type is JSON using fluent assertions
        response?.Content.Headers.ContentType?.MediaType
            .Should().Be("application/json");

    }
}
```
6. **Let's create a new integration test**: This time to validate the response. Write this comment at the end of your integration tests file: _// Let's create an integration test to check the serialization of the response into OpenMeteoResponse_ and wait for Copilot to suggest.
```csharp
[Fact]
public async Task Get_ReturnsWeatherForecasts_PropertlyDeserialized()
{
    // Arrange
    var client = _factory.CreateClient();
    var latitude = 40.4165;
    var longitude = -3.70256;

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
```
7. **Let's use a theorem to test with a set of different latitude and longitude**: Select the suggested tests and Ask Copilot: _/tests Could you please refactor this test using a Theory and latitude and longitude for Madrid, Málag, Barcelona, Sevilla y Coruña?_
```csharp
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
```
8. **Ask Copilot to explain the code a little**: Select the new test and use the prompt _/explain_ to get a brief explanation of the test.

## Create code from mermaid diagram
We recommend you to start from the Intermediate Solution.
1. **Have a look at** the src\Controllers, src\Interfaces and src\Dtos
2. **Locate files** BookTravelController.cs, CarRentalInformation.cs, HotelBookingInformation.cs, ICarRental.cs and IHotelBooking.cs
3. **Check the mermaid sequence diagram** (You may use [Mermaid Live](https://mermaid.live))
```
sequenceDiagram
    participant User
    participant BookTravelController
    participant IWeatherService
    participant IHotelBooking
    participant ICarRental
    
    User->>BookTravelController: Ask weather for Barcelona on 5/8/2025
    activate BookTravelController
    BookTravelController->>IWeatherService: Ask weather for Barcelona on 5/8/2025
    activate IWeatherService
    IWeatherService-->>BookTravelController: Return weather (sunny)
    deactivate IWeatherService    
    BookTravelController->>IHotelBooking: Request hotel information for Barcelona on 5/8/2025
    activate IHotelBooking
    IHotelBooking-->>BookTravelController: Returns information(IEnumerable<HotelBookingInfo>)
    deactivate IHotelBooking
    BookTravelController->>ICarRental: Request car information for Barcelona on 5/8/2025
    activate ICarRental
    ICarRental-->>BookTravelController: Returns information(IEnumerable<CarBookingInfo>)
    deactivate ICarRental    
    BookTravelController-->>User: Returns information(sunny, IEnumerable<HotelBookingInfo>, IEnumerable<CarBookingInfo>)
    deactivate BookTravelController
```
4. **Open GitHub Copilot chat** And try the prompt: _Hello, Copilot. Please, in my #file:BookTravelController.cs I need to implement the code according to the next sequence diagram written using mermaid._ 

Here you may copy and paste the sequenceDiagram above or even just select the sequence diagram in this README.md file.

5. **Copilot should give you a piece of code similar to next**
_Based on the sequence diagram you provided, you need to implement a method in your BookTravelController that interacts with IWeatherService, IHotelBooking, and ICarRental services._

_Here is the pseudocode for the method:_

_Define a method named GetTravelInfo that takes in a location and a date as parameters._

_- Call the GetWeather method of the IWeatherService with the location and date as arguments and store the result._

_- Call the GetHotelBookingInfo method of the IHotelBooking with the location and date as arguments and store the result._

_- Call the GetCarRentalInfo method of the ICarRental with the location and date as arguments and store the result._

_- Return a response containing the weather, hotel booking info, and car rental info.
Here is the C# code for the GetTravelInfo method:_

```csharp
[HttpGet]
public async Task<IActionResult> GetTravelInfo(string location, DateTime date)
{
    var weather = await _weatherService.GetWeather(location, date);
    var hotelBookingInfo = await _hotelBooking.GetHotelBookingInfo(location, date);
    var carRentalInfo = await _carRental.GetCarRentalInfo(location, date);

    return Ok(new { weather, hotelBookingInfo, carRentalInfo });
}
```
_Please note that this code assumes that the IWeatherService, IHotelBooking, and ICarRental interfaces have the GetWeather, GetHotelBookingInfo, and GetCarRentalInfo methods respectively, and that these methods are asynchronous and return a Task. If this is not the case, you will need to adjust the code accordingly._
   
## Some breakdowns
From time to time, you may find some prompts that lead you to a dead end. Let's explore an example:

### Public code 
1. **Let's try the prompt**: _Hello, Copilot. Could you create a Dictionary with 20 different real countries being the keys and 20 different real cities per country?_
    1. **The result**: _Sorry, the response matched public code, which is blocked by your organization. Try rephrasing your prompt._

### How we solved it?
1. **Let's change a little the prompt approach**: _Hello, Copilot. I need to create a Dictionary of 20 cities, aggregatd by country. Can you help me with it?_
```cs
Dictionary<string, List<string>> countryCities = new Dictionary<string, List<string>>();

// Add cities to the USA
countryCities["USA"] = new List<string>() { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville", "Fort Worth", "Columbus", "San Francisco", "Charlotte", "Indianapolis", "Seattle", "Denver", "Washington" };

// Add cities to the UK
countryCities["UK"] = new List<string>() { "London", "Birmingham", "Manchester", "Glasgow", "Leeds", "Liverpool", "Newcastle", "Sheffield", "Bristol", "Edinburgh", "Leicester", "Belfast", "Brighton", "Bournemouth", "Cardiff", "Middlesbrough", "Stoke-on-Trent", "Coventry", "Sunderland", "Bradford" };

// Add cities to Australia
countryCities["Australia"] = new List<string>() { "Sydney", "Melbourne", "Brisbane", "Perth", "Adelaide", "Gold Coast", "Canberra", "Newcastle", "Wollongong", "Logan City", "Geelong", "Hobart", "Townsville", "Cairns", "Darwin", "Toowoomba", "Ballarat", "Bendigo", "Albury", "Launceston" };
```

2. **Let's refine a little**: _Could you expand the list up to 20 different countries?_
```cs
Dictionary<string, List<string>> countryCities = new Dictionary<string, List<string>>();

// Add cities to the USA
countryCities["USA"] = new List<string>() { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" };
// Add cities to the UK
countryCities["UK"] = new List<string>() { "London", "Birmingham", "Manchester", "Glasgow", "Leeds" };
// Add cities to Australia
countryCities["Australia"] = new List<string>() { "Sydney", "Melbourne", "Brisbane", "Perth", "Adelaide" };
// Add cities to Canada
// The rest of the countries went here...
```

1. **One more step**: _/fix Could you please refactor this code so the Dictionary is initialized when the instance is created?_
```cs
public static Dictionary<string, List<string>> CountryCities2 = new Dictionary<string, List<string>>
{
    ["USA"] = new List<string>() { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" },
    ["UK"] = new List<string>() { "London", "Birmingham", "Manchester", "Glasgow", "Leeds" },
    ["Australia"] = new List<string>() { "Sydney", "Melbourne", "Brisbane", "Perth", "Adelaide" },
    // Add the rest of the countries here...
};
```
**Note**: In some scenarios you may prefer to code it yourself instead of keep on looping with the prompt refinement. We decided to include the code in Constants.cs



---
This README.md file has been completed with the aid of GitHub Copilot