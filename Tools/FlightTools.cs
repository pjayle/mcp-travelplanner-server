using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace TravelMcp.Server.Tools;

/// <summary>Mock flight search. Replace the body with Amadeus / Skyscanner API calls.</summary>
[McpServerToolType]
public class FlightTools
{
    private static readonly string[] Airlines = ["Japan Airlines", "ANA", "Singapore Airlines", "Air India", "Emirates", "Qatar Airways"];

    [McpServerTool(Name = "search_flights"),
     Description("Search round-trip flights between two cities. Returns airline, times, stops, duration and price in USD.")]
    public static string SearchFlights(
        [Description("Origin city or IATA code")] string origin,
        [Description("Destination city or IATA code")] string destination,  
        [Description("Departure date yyyy-MM-dd")] string departDate,
        [Description("Return date yyyy-MM-dd")] string returnDate)
    {
        var rng = Stable.Rng(origin, destination, departDate);
        var options = Enumerable.Range(0, 5).Select(_ =>
        {
            var stops = rng.Next(0, 3);
            var hours = 6 + stops * 3 + rng.Next(0, 4);
            var dep = TimeSpan.FromMinutes(rng.Next(5, 22) * 60 + rng.Next(0, 4) * 15);
            return new
            {
                airline = Airlines[rng.Next(Airlines.Length)],
                departs = $"{departDate} {dep:hh\\:mm}",
                stops,
                durationHours = hours,
                returnDate,
                priceUsd = 350 + hours * rng.Next(25, 55) - (stops == 0 ? 0 : 60)
            };
        }).OrderBy(f => f.priceUsd).ToList();

        return JsonSerializer.Serialize(new { origin, destination, results = options });
    }
}
