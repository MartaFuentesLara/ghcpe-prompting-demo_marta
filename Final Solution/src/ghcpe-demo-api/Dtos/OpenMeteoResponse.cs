/**
 * Open meteo response is a json object that contains the weather forecast data
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
 */

/// <summary>
/// Weather forecast DTO from the Open Meteo API
/// </summary>
/// <remarks>The next three records were created by Copilot<remarks>
public class OpenMeteoResponse
{
    public Current? Current { get; set; }
    public Hourly? Hourly { get; set; }
}

public class Current
{
    public DateTime Time { get; set; }
    public double Temperature_2m { get; set; }
    public double Wind_Speed_10m { get; set; }
}

public class Hourly
{
    public DateTime[] Time { get; set; } = [];
    public double[] Wind_Speed_10m { get; set; } = [];
    public double[] Temperature_2m { get; set; } = [];
    public int[] Relative_Humidity_2m { get; set; } = [];
}
