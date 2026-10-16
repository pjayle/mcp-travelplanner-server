using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace TravelMcp.Server.Tools;

/// <summary>Real weather via Open-Meteo (free, no API key).</summary>
[McpServerToolType]
public class WeatherTools
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };

    [McpServerTool(Name = "get_weather_forecast"),
     Description("Daily weather forecast (min/max temp °C, rain probability, conditions) for a city. Max 16 days ahead.")]
    public static async Task<string> GetWeatherForecast(
        [Description("City name, e.g. 'Tokyo'")] string city,
        [Description("Number of days (1-16)")] int days = 5)
    {
        days = Math.Clamp(days, 1, 16);

        var geoJson = await Http.GetStringAsync(
            $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1");
        using var geo = JsonDocument.Parse(geoJson);
        if (!geo.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            return JsonSerializer.Serialize(new { error = $"City '{city}' not found" });

        var place = results[0];
        var lat = place.GetProperty("latitude").GetDouble();
        var lon = place.GetProperty("longitude").GetDouble();
        var name = place.GetProperty("name").GetString();
        var country = place.TryGetProperty("country", out var c) ? c.GetString() : "";

        var url = "https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat}&longitude={lon}" +
                  "&daily=temperature_2m_max,temperature_2m_min,precipitation_probability_max,weathercode" +
                  $"&timezone=auto&forecast_days={days}";
        using var fc = JsonDocument.Parse(await Http.GetStringAsync(url));
        var daily = fc.RootElement.GetProperty("daily");

        var list = new List<object>();
        for (var i = 0; i < daily.GetProperty("time").GetArrayLength(); i++)
        {
            var rain = daily.GetProperty("precipitation_probability_max")[i];
            list.Add(new
            {
                date = daily.GetProperty("time")[i].GetString(),
                minC = daily.GetProperty("temperature_2m_min")[i].GetDouble(),
                maxC = daily.GetProperty("temperature_2m_max")[i].GetDouble(),
                rainChancePercent = rain.ValueKind == JsonValueKind.Null ? 0 : rain.GetInt32(),
                conditions = Describe(daily.GetProperty("weathercode")[i].GetInt32())
            });
        }
        return JsonSerializer.Serialize(new { location = $"{name}, {country}", forecast = list });
    }

    private static string Describe(int code) => code switch
    {
        0 => "Clear sky",
        1 or 2 or 3 => "Partly cloudy",
        45 or 48 => "Fog",
        >= 51 and <= 57 => "Drizzle",
        >= 61 and <= 67 => "Rain",
        >= 71 and <= 77 => "Snow",
        >= 80 and <= 82 => "Rain showers",
        >= 95 => "Thunderstorm",
        _ => "Mixed"
    };
}
