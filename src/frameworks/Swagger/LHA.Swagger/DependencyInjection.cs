using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace LHA.Swagger;

/// <summary>
/// Extension methods for registering the LHA Swagger / OpenAPI services.
/// </summary>
public static class SwaggerServiceCollectionExtensions
{
    /// <summary>
    /// Registers OpenAPI document generation, security transformers, and Swagger/Scalar UI
    /// using centralized configuration from the <c>"Swagger"</c> config section.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">Application configuration (for binding options).</param>
    /// <param name="configureOptions">
    /// Optional delegate to override options after binding from configuration.
    /// Use this for per-service customization such as title, description, version.
    /// </param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddLHASwagger(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<LhaSwaggerOptions>? configureOptions = null)
    {
        // ── Bind options ─────────────────────────────────────────────
        var section = configuration.GetSection(LhaSwaggerOptions.SectionName);
        services.Configure<LhaSwaggerOptions>(section);

        if (configureOptions is not null)
            services.PostConfigure(configureOptions);

        // Read options eagerly for registration decisions
        var opts = new LhaSwaggerOptions();
        section.Bind(opts);
        configureOptions?.Invoke(opts);

        if (!opts.Enabled)
            return services;

        // ── Register OpenAPI documents ───────────────────────────────
        if (opts.Documents is { Count: > 0 })
        {
            foreach (var doc in opts.Documents)
            {
                services.AddOpenApi(doc.Name, o => ConfigureOpenApiOptions(o, opts));
            }
        }
        else
        {
            // Single document using the version name
            services.AddOpenApi(opts.Version, o => ConfigureOpenApiOptions(o, opts));
        }

        // ── Register Swashbuckle (for SwaggerUI) ─────────────────────
        if (opts.UiProvider is SwaggerUiProvider.SwaggerUi or SwaggerUiProvider.Both)
        {
            services.AddSwaggerGen();
        }

        return services;
    }

    private static void ConfigureOpenApiOptions(OpenApiOptions o, LhaSwaggerOptions opts)
    {
        // Register document transformers
        o.AddDocumentTransformer<LhaDocumentInfoTransformer>();
        o.AddDocumentTransformer<LhaDocumentSecurityTransformer>();

        // Register operation transformer for per-endpoint security
        o.AddOperationTransformer<LhaOperationSecurityTransformer>();
    }
}

/// <summary>
/// Extension methods for configuring the LHA Swagger / OpenAPI middleware pipeline.
/// </summary>
public static class SwaggerApplicationBuilderExtensions
{
    /// <summary>
    /// Maps the OpenAPI JSON endpoint(s) and the configured UI provider(s).
    /// Respects <see cref="LhaSwaggerOptions.Enabled"/> and
    /// <see cref="LhaSwaggerOptions.AllowedEnvironments"/>.
    /// </summary>
    public static WebApplication UseLHASwagger(this WebApplication app)
    {
        var opts = app.Configuration
            .GetSection(LhaSwaggerOptions.SectionName)
            .Get<LhaSwaggerOptions>() ?? new LhaSwaggerOptions();

        if (!opts.Enabled)
            return app;

        // Environment gate
        if (opts.AllowedEnvironments is { Length: > 0 })
        {
            var envName = app.Environment.EnvironmentName;
            var allowed = opts.AllowedEnvironments
                .Any(e => string.Equals(e, envName, StringComparison.OrdinalIgnoreCase));

            if (!allowed)
                return app;
        }

        // ── Map OpenAPI JSON endpoints ───────────────────────────────
        if (opts.Documents is { Count: > 0 })
        {
            foreach (var doc in opts.Documents)
            {
                app.MapOpenApi($"/{opts.OpenApiRoutePrefix}/{{documentName}}.json");
            }
        }
        else
        {
            app.MapOpenApi($"/{opts.OpenApiRoutePrefix}/{{documentName}}.json");
        }

        // ── Map UI ───────────────────────────────────────────────────
        var documentNames = opts.Documents is { Count: > 0 }
            ? opts.Documents.Select(d => d.Name).ToArray()
            : [opts.Version];

        switch (opts.UiProvider)
        {
            case SwaggerUiProvider.Scalar:
                MapScalar(app, opts, documentNames);
                break;

            case SwaggerUiProvider.SwaggerUi:
                MapSwaggerUi(app, opts, documentNames);
                break;

            case SwaggerUiProvider.Both:
                MapScalar(app, opts, documentNames);
                MapSwaggerUi(app, opts, documentNames);
                break;

            case SwaggerUiProvider.None:
                break;
        }

        return app;
    }

    private static void MapScalar(
        WebApplication app,
        LhaSwaggerOptions opts,
        string[] documentNames)
    {
        app.MapScalarApiReference($"/{opts.ScalarRoutePrefix}", o =>
        {
            o.Title = opts.Title;
            o.OpenApiRoutePattern = $"/{opts.OpenApiRoutePrefix}/{{documentName}}.json";
        });
    }

    private static void MapSwaggerUi(
        WebApplication app,
        LhaSwaggerOptions opts,
        string[] documentNames)
    {
        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = opts.SwaggerUiRoutePrefix;

            foreach (var name in documentNames)
            {
                c.SwaggerEndpoint(
                    $"/{opts.OpenApiRoutePrefix}/{name}.json",
                    $"{opts.Title} — {name}");
            }

            c.DocumentTitle = $"{opts.Title} — Swagger UI";
            c.DefaultModelsExpandDepth(-1);
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
    }
}
