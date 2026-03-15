using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// 1. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("X-Debug-Culture", "X-App-Locale");
    });
});

var app = builder.Build();

// 2. Enable CORS
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 3. Define Endpoints matching ExampleApiClient

// GET api/weather
var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
app.MapGet("api/weather", () =>
{
    return Enumerable.Range(1, 5).Select(index => new WeatherForecast(
        DateTime.Now.AddDays(index),
        Random.Shared.Next(-20, 55),
        summaries[Random.Shared.Next(summaries.Length)]
    )).ToArray();
});

// GET api/users/{id}
app.MapGet("api/users/{id}", (int id) => 
{
    return Results.Ok(new UserDto(id, $"User {id}", $"user{id}@example.com"));
});

// POST api/users
app.MapPost("api/users", ([FromBody] CreateUserRequest request) => 
{
    return Results.Created($"/api/users/1", new UserDto(1, request.Name, request.Email));
});

// PUT api/users/{id}
app.MapPut("api/users/{id}", (int id, [FromBody] UpdateUserRequest request) => 
{
    return Results.Ok(new UserDto(id, request.Name, $"user{id}@example.com"));
});

// DELETE api/users/{id}
app.MapDelete("api/users/{id}", (int id) => 
{
    return Results.Ok($"User {id} deleted successfully.");
});

// --- Error Test Endpoints ---

// 404 Not Found
app.MapGet("api/test/not-found", () => Results.NotFound(new { Message = "The requested resource was not found." }));

// 500 Server Error
app.MapGet("api/test/server-error", () => 
{
    throw new Exception("This is a simulated internal server error.");
});

// 401 Unauthorized
app.MapGet("api/test/unauthorized", () => Results.Unauthorized());

app.Run();

// Dummy models used only for demonstration purposes
public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary);
public record UserDto(int Id, string Name, string Email);
public record CreateUserRequest(string Name, string Email);
public record UpdateUserRequest(string Name);
