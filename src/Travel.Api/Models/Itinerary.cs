namespace Travel.Api.Models;

public class Itinerary
{
    public int Id { get; set; }
    public string TripName { get; set; } = default!;
    public string Destination { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public int Days { get; set; }
    public string InterestsCsv { get; set; } = string.Empty;
    public string BudgetLevel { get; set; } = "mid";
    public string LlmPlanMarkdown { get; set; } = string.Empty;
    public string PlacesJson { get; set; } = "[]";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
