using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Travel.Api.Services;

public class OpenAiService
{
    private readonly HttpClient _http;
    private readonly string? _apiKey;

    public OpenAiService(IHttpClientFactory factory, IConfiguration cfg)
    {
        _http = factory.CreateClient();
        _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? cfg["OPENAI_API_KEY"];
    }

    public async Task<string> GeneratePlanAsync(string destination, int days, IEnumerable<string> interests, string budget)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "OpenAI key missing. Set OPENAI_API_KEY in environment.";

        var prompt = $@"
You are a travel planner. Create a {days}-day itinerary for {destination}.
Consider interests: {string.Join(", ", interests)}. Budget: {budget}.
Return a clear day-by-day plan with morning/afternoon/evening, plus brief tips.
Keep it concise and markdown-friendly.
";

        var req = new
        {
            model = "gpt-4o-mini",
            messages = new[] { new { role = "user", content = prompt } },
            temperature = 0.7
        };

        var msg = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        msg.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

        var resp = await _http.SendAsync(msg);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            return $"OpenAI error: {resp.StatusCode} - {body}";

        using var doc = JsonDocument.Parse(body);
        var content = doc.RootElement.GetProperty("choices")[0]
            .GetProperty("message").GetProperty("content").GetString();

        return content ?? "Itinerary generation failed.";
    }
}
