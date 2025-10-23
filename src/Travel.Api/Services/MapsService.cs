using System.Text.Json;

namespace Travel.Api.Services;

public class MapsService
{
    private readonly HttpClient _http;
    private readonly string? _mapsKey;

    public MapsService(IHttpClientFactory factory, IConfiguration cfg)
    {
        _http = factory.CreateClient();
        _mapsKey = Environment.GetEnvironmentVariable("MAPS_API_KEY") ?? cfg["MAPS_API_KEY"];
    }

    public async Task<List<object>> GetTopPlacesAsync(string destination, int max = 8)
    {
        if (string.IsNullOrWhiteSpace(_mapsKey))
            return new List<object> { new { warning = "MAPS_API_KEY not set." } };

        var q = Uri.EscapeDataString(destination + " top sights");
        var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={q}&key={_mapsKey}";
        var json = await _http.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);
        var list = new List<object>();
        if (!doc.RootElement.TryGetProperty("results", out var results)) return list;

        foreach (var r in results.EnumerateArray().Take(max))
        {
            string? name = r.TryGetProperty("name", out var n) ? n.GetString() : null;
            string? address = r.TryGetProperty("formatted_address", out var a) ? a.GetString() : null;
            double? rating = r.TryGetProperty("rating", out var rt) ? rt.GetDouble() : (double?)null;
            string? pid = r.TryGetProperty("place_id", out var p) ? p.GetString() : null;

            list.Add(new { name, address, rating, place_id = pid });
        }
        return list;
    }
}
