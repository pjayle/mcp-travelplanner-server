using System.Security.Cryptography;
using System.Text;

namespace TravelMcp.Server.Tools;

/// <summary>Deterministic pseudo-random numbers so mock data is stable between calls.</summary>
internal static class Stable
{
    public static Random Rng(params string[] parts)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(string.Join('|', parts).ToLowerInvariant()));
        return new Random(BitConverter.ToInt32(hash, 0));
    }
}
