using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel.Api.Data;
using Travel.Api.Dtos;
using Travel.Api.Models;
using Travel.Api.Services;
using System.Text.Json;

namespace Travel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItinerariesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly OpenAiService _llm;
    private readonly MapsService _maps;

    public ItinerariesController(AppDbContext db, OpenAiService llm, MapsService maps)
    {
        _db = db; _llm = llm; _maps = maps;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Itinerary>>> List() =>
        await _db.Itineraries.OrderByDescending(i => i.CreatedUtc).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Itinerary>> Get(int id) =>
        await _db.Itineraries.FindAsync(id) is { } it ? it : NotFound();

    [HttpPost("generate")]
    public async Task<ActionResult<Itinerary>> Generate([FromBody] GenerateItineraryRequest req)
    {
        var plan = await _llm.GeneratePlanAsync(req.Destination, req.Days, req.Interests ?? new List<string>(), req.BudgetLevel);
        var places = await _maps.GetTopPlacesAsync(req.Destination);

        var itinerary = new Itinerary
        {
            TripName = req.TripName,
            Destination = req.Destination,
            StartDate = req.StartDate,
            Days = req.Days,
            InterestsCsv = string.Join(",", req.Interests ?? new()),
            BudgetLevel = req.BudgetLevel,
            LlmPlanMarkdown = plan,
            PlacesJson = JsonSerializer.Serialize(places)
        };

        _db.Itineraries.Add(itinerary);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = itinerary.Id }, itinerary);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Itinerary updated)
    {
        if (id != updated.Id) return BadRequest();
        _db.Entry(updated).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var it = await _db.Itineraries.FindAsync(id);
        if (it is null) return NotFound();
        _db.Itineraries.Remove(it);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
