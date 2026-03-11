using Microsoft.Extensions.DependencyInjection;
using System;
using Blazored.LocalStorage;

namespace LHA.BlazorWasm.Services.Storage;

/// <summary>
/// Dependency Injection extension methods for the Local Storage service.
/// </summary>
public static class StorageExtensions
{
    /// <summary>
    /// Adds the application's abstracted local storage service with default or configured prefix options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional configuration action to customize StorageOptions.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddAppLocalStorage(
        this IServiceCollection services,
        Action<StorageOptions>? configureOptions = null)
    {
        // Add the underlying library
        services.AddBlazoredLocalStorage();

        // Configure options
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<StorageOptions>(options => { /* use defaults */ });
        }

        // Register our abstraction
        services.AddScoped<ILocalStorageService, LocalStorageService>();

        return services;
    }
}
