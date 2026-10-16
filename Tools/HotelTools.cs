using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace TravelMcp.Server.Tools;

/// <summary>Mock hotel search. Replace the body with Amadeus / Booking / Expedia API calls.</summary>
[McpServerToolType]
public class HotelTools
{
    private static readonly string[] Names =
        ["Grand {0} Hotel", "{0} Central Inn", "The {0} Residence", "{0} Garden Suites", "Sakura {0} Boutique", "{0} Skyline Tower"];
    private static readonly string[] Areas = ["City Center", "Old Town", "Station District", "Waterfront", "Business District"];

    [McpServerTool(Name = "search_hotels"),
     Description("Search hotels in a city. Returns name, area, star rating, price per night in USD and total price.")]
    public static string SearchHotels(
        [Description("City, e.g. 'Tokyo'")] string city,
        [Description("Check-in date yyyy-MM-dd")] string checkIn,
        [Description("Number of nights")] int nights = 1,
        [Description("Max price per night in USD, 0 = no limit")] int maxPricePerNight = 0)
    {
        var rng = Stable.Rng(city, checkIn);
        var hotels = Names.Select(n =>
        {
            var stars = rng.Next(3, 6);
            var price = stars * rng.Next(35, 70);
            return new
            {
                name = string.Format(n, city),
                area = Areas[rng.Next(Areas.Length)],
                stars,
                rating = Math.Round(7 + rng.NextDouble() * 2.8, 1),
                pricePerNightUsd = price,
                totalUsd = price * Math.Max(1, nights),
                freeCancellation = rng.Next(2) == 1
            };
        })
        .Where(h => maxPricePerNight <= 0 || h.pricePerNightUsd <= maxPricePerNight)
        .OrderByDescending(h => h.rating)
        .Take(5)
        .ToList();

        return JsonSerializer.Serialize(new { city, checkIn, nights, results = hotels });
    }
}
