using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Core;
using LHA.BlazorWasm.HttpApi.Client.Handlers;
using LHA.BlazorWasm.HttpApi.Client.Options;
using LHA.BlazorWasm.HttpApi.Client.Clients;
using LHA.Shared.Contracts.AuditLog;
using LHA.Security.Encryption;
using LHA.Security.Keys;
using LHA.Security.Signing;
using LHA.Security.Device;
using LHA.BlazorWasm.HttpApi.Client.Clients.PermissionManagement;

namespace LHA.BlazorWasm.HttpApi.Client.Extensions;

public static class HttpApiClientExtensions
{
    /// <summary>
    /// Registers the HTTP API client infrastructure, configuring standard pipeline handlers.
    /// </summary>
    public static IServiceCollection AddLhaHttpApiClient(
        this IServiceCollection services,
        Action<HttpApiClientOptions> configureOptions)
    {
        // 1. Configure Options
        services.Configure(configureOptions);

        // 2. Register Shared Services
        services.AddTransient<IApiErrorHandler, DefaultApiErrorHandler>();
        services.AddTransient<IClientContextProvider, DefaultClientContextProvider>();

        // Security primitives
        services.AddSingleton<IAesEncryptionService, AesEncryptionService>();
        services.AddSingleton<IKeyRotationService, KeyRotationService>();
        services.AddSingleton<IRequestSigner, RequestSigner>();
        services.AddSingleton<IDeviceFingerprintService, DeviceFingerprintService>();

        // 3. Register Pipeline Handlers
        services.AddTransient<LoggingMessageHandler>();

        services.AddTransient(sp =>
        {
            var options = sp.GetRequiredService<IOptions<HttpApiClientOptions>>().Value;
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<RetryMessageHandler>>();
            return new RetryMessageHandler(logger, options.MaxRetries);
        });

        services.AddTransient<SecureHttpHandler>();
        services.AddTransient<ContextMessageHandler>();
        services.AddTransient<AuthMessageHandler>();

        // 4. Register Typed Clients (extend this list as new clients are created)
        RegisterTypedClient<AuditLogApiClient>(services);
        services.AddScoped<IAuditLogAppService>(sp => sp.GetRequiredService<AuditLogApiClient>());

        RegisterTypedClient<AuthApiClient>(services);
        RegisterTypedClient<RoleApiClient>(services);
        RegisterTypedClient<UserApiClient>(services);
        RegisterTypedClient<PermissionApiClient>(services);
        RegisterTypedClient<PermissionGroupApiClient>(services);

        return services;
    }

    private static IHttpClientBuilder RegisterTypedClient<TClient>(IServiceCollection services)
        where TClient : class, IApiClient
    {
        return services.AddHttpClient<TClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<HttpApiClientOptions>>().Value;
                if (!string.IsNullOrEmpty(options.BaseAddress))
                {
                    client.BaseAddress = new Uri(options.BaseAddress);
                }

                client.Timeout = options.Timeout;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            // Handlers are executed top-to-bottom (outermost to innermost)
            // Logging wraps Retry, Retry wraps Context, Context wraps Auth
            .AddHttpMessageHandler<LoggingMessageHandler>()
            .AddHttpMessageHandler<RetryMessageHandler>()
            .AddHttpMessageHandler<ContextMessageHandler>()
            .AddHttpMessageHandler<AuthMessageHandler>();
    }
}
