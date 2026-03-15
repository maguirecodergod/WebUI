using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;

namespace LHA.BlazorWasm.HttpApi.Client.Clients;

/// <summary>
/// A strongly-typed example client demonstrating standard REST operations.
/// </summary>
public class ExampleApiClient : ApiClientBase
{
    public ExampleApiClient(HttpClient httpClient, IApiErrorHandler errorHandler) 
        : base(httpClient, errorHandler)
    {
    }

    public Task<ApiResponse<WeatherForecast[]>> GetWeatherAsync(CancellationToken cancellationToken = default)
    {
        return GetAsync<WeatherForecast[]>("api/weather", cancellationToken: cancellationToken);
    }

    public Task<ApiResponse<UserDto>> GetUserAsync(int id, CancellationToken cancellationToken = default)
    {
        // Example of adding a custom header for a specific request
        return GetAsync<UserDto>($"api/users/{id}", 
            request => request.Headers.Add("X-Source", "WeatherPage"), 
            cancellationToken);
    }

    public Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        return PostAsync<CreateUserRequest, UserDto>("api/users", request, cancellationToken: cancellationToken);
    }
}

// Dummy models used only for demonstration purposes
public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary);
public record UserDto(int Id, string Name, string Email);
public record CreateUserRequest(string Name, string Email);
