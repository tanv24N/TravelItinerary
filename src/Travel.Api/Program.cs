using Microsoft.EntityFrameworkCore;
using Travel.Api.Data;
using Travel.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// SQLite only for GitHub-only local/Codespaces setup
var cs = builder.Configuration.GetConnectionString("Default")
         ?? builder.Configuration["ConnectionStrings__Default"]
         ?? "Data Source=itineraries.db";

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite(cs));

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("allow-local", p =>
    p.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5173", "http://localhost:3000")));

builder.Services.AddScoped<OpenAiService>();
builder.Services.AddScoped<MapsService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("allow-local");
app.MapControllers();

// ensure DB for quick start
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
