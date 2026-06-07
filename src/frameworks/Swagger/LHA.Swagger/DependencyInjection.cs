using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        if (opts.UiProvider is SwaggerUiProvider.SwaggerUi or SwaggerUiProvider.All)
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

        // Register operation transformers — security first, then documentation
        o.AddOperationTransformer<LhaOperationSecurityTransformer>();
        o.AddOperationTransformer<LhaOperationDocumentationTransformer>();

        // Register schema transformer for XML comments and enum metadata
        o.AddSchemaTransformer<LhaSchemaDocumentationTransformer>();
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

            case SwaggerUiProvider.All:
                MapScalar(app, opts, documentNames);
                MapSwaggerUi(app, opts, documentNames);
                break;

            case SwaggerUiProvider.Redoc:
                MapRedoc(app, opts, documentNames);
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
        app.MapScalarApiReference($"/{opts.ScalarRoutePrefix}", (o, context) =>
        {
            var requestOrigin = GetPublicRequestOrigin(context);

            o.Title = opts.Title;
            o.OpenApiRoutePattern = $"/{opts.OpenApiRoutePrefix}/{{documentName}}.json";

            if (!opts.Scalar.ShowClientButton)
            {
                o.HideClientButton();
            }

            ConfigureDeveloperTools(o, opts.Scalar.ShowDeveloperTools);

            foreach (var server in opts.Servers.Where(s => !string.IsNullOrWhiteSpace(s.Url)))
            {
                o.AddServer(server.Url, server.Description ?? server.Url);
            }

            if (opts.Scalar.UseRequestOriginAsServer)
            {
                o.AddServer(requestOrigin, "Current environment");
            }

            foreach (var preference in opts.Scalar.DefaultHttpClients)
            {
                if (TryParseScalarHttpClientPreference(preference, out var target, out var client))
                {
                    o.WithDefaultHttpClient(target, client);
                }
            }

            var mcp = opts.Scalar.McpServer;
            if (!mcp.Enabled)
            {
                o.DisableMcp();
                return;
            }

            var mcpName = string.IsNullOrWhiteSpace(mcp.Name) ? opts.Title : mcp.Name;
            var mcpUrl = ResolvePublicUrl(requestOrigin, mcp.Url, mcp.Path);

            if (!string.IsNullOrWhiteSpace(mcpUrl))
            {
                o.WithMcpServer(mcpName, mcpUrl);
            }
        });
    }

    private static void ConfigureDeveloperTools(
        ScalarOptions options,
        LhaScalarDeveloperToolsVisibility visibility)
    {
        switch (visibility)
        {
            case LhaScalarDeveloperToolsVisibility.Always:
                options.AlwaysShowDeveloperTools();
                break;

            case LhaScalarDeveloperToolsVisibility.Never:
                options.HideDeveloperTools();
                break;

            case LhaScalarDeveloperToolsVisibility.Localhost:
                break;
        }
    }

    private static string GetPublicRequestOrigin(HttpContext context)
    {
        var scheme = GetForwardedHeaderValue(context, "X-Forwarded-Proto")
                     ?? context.Request.Scheme;

        var host = GetForwardedHeaderValue(context, "X-Forwarded-Host")
                   ?? context.Request.Host.Value;

        return $"{scheme}://{host}".TrimEnd('/');
    }

    private static string? GetForwardedHeaderValue(HttpContext context, string headerName)
    {
        var value = context.Request.Headers[headerName].FirstOrDefault();
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Split(',')[0].Trim();
    }

    private static string ResolvePublicUrl(string requestOrigin, string? configuredUrl, string fallbackPath)
    {
        var value = string.IsNullOrWhiteSpace(configuredUrl) ? fallbackPath : configuredUrl;

        if (Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            return value;
        }

        return $"{requestOrigin}/{value.TrimStart('/')}";
    }

    private static bool TryParseScalarHttpClientPreference(
        ScalarHttpClientPreference preference,
        out ScalarTarget target,
        out ScalarClient client)
    {
        target = default;
        client = default;

        return Enum.TryParse(preference.Target, ignoreCase: true, out target) &&
               Enum.TryParse(preference.Client, ignoreCase: true, out client);
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

    private static void MapRedoc(
        WebApplication app,
        LhaSwaggerOptions opts,
        string[] documentNames)
    {
        app.MapGet($"/{opts.RedocRoutePrefix}", async context =>
        {
            var docName = documentNames.Length > 0 ? documentNames[0] : opts.Version;
            var specUrl = $"/{opts.OpenApiRoutePrefix}/{docName}.json";

            var html = $$"""
                <!DOCTYPE html>
                <html>
                <head>
                    <title>{{opts.Title}} — ReDoc</title>
                    <meta charset="utf-8"/>
                    <meta name="viewport" content="width=device-width, initial-scale=1">
                    <link href="https://fonts.googleapis.com/css?family=Montserrat:300,400,700|Roboto:300,400,700" rel="stylesheet">
                    <style>
                        body { margin: 0; padding: 0; }
                        redoc { display: block; }
                    </style>
                    <script src="https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js"></script>
                </head>
                <body>
                    <redoc spec-url="{{specUrl}}"></redoc>
                </body>
                </html>
            """;
            context.Response.ContentType = "text/html";
            await context.Response.WriteAsync(html);
        });
    }
}
