using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TravelMcp.Server.Tools;

// One executable, three independent MCP servers (stdio transport).
// Usage: TravelMcp.Server.dll weather | hotel | flight
var which = args.FirstOrDefault()?.ToLowerInvariant() ?? "weather";

var builder = Host.CreateApplicationBuilder(args);

// IMPORTANT: stdout is the MCP protocol channel. All logs must go to stderr.
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

var mcp = builder.Services.AddMcpServer().WithStdioServerTransport();
switch (which)
{
    case "weather": mcp.WithTools<WeatherTools>(); break;
    case "hotel":   mcp.WithTools<HotelTools>();   break;
    case "flight":  mcp.WithTools<FlightTools>();  break;
    default: throw new ArgumentException($"Unknown server '{which}'. Use weather|hotel|flight");
}

await builder.Build().RunAsync();
