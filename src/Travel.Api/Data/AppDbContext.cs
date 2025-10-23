using Microsoft.EntityFrameworkCore;
using Travel.Api.Models;

namespace Travel.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Itinerary> Itineraries => Set<Itinerary>();

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<Itinerary>();
        foreach (var e in entries)
        {
            if (e.State == EntityState.Added)
                e.Entity.CreatedUtc = DateTime.UtcNow;
            e.Entity.UpdatedUtc = DateTime.UtcNow;
        }
    }
}
