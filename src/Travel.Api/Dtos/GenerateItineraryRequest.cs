namespace Travel.Api.Dtos;

public class GenerateItineraryRequest
{
    public string TripName { get; set; } = "My Trip";
    public string Destination { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public int Days { get; set; } = 3;
    public List<string>? Interests { get; set; } = new() { "food", "museums" };
    public string BudgetLevel { get; set; } = "mid";
}
