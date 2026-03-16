using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace LHA.Swagger;

// ─────────────────────────────────────────────────────────────────────
// 1. Injects metadata, contact, license, servers, external docs
// ─────────────────────────────────────────────────────────────────────

/// <summary>
/// Transforms the OpenAPI document to apply centralized info metadata,
/// contact, license, servers, external docs, and custom extension properties
/// defined in <see cref="LhaSwaggerOptions"/>.
/// </summary>
internal sealed class LhaDocumentInfoTransformer(
    IOptions<LhaSwaggerOptions> options) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var opts = options.Value;

        // ── Title & description ──────────────────────────────────────
        document.Info.Title = opts.Title;
        document.Info.Version = opts.Version;

        if (!string.IsNullOrWhiteSpace(opts.Description))
            document.Info.Description = opts.Description;

        // ── Terms of service ─────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(opts.TermsOfServiceUrl))
            document.Info.TermsOfService = new Uri(opts.TermsOfServiceUrl);

        // ── Contact ──────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(opts.ContactName) ||
            !string.IsNullOrWhiteSpace(opts.ContactEmail) ||
            !string.IsNullOrWhiteSpace(opts.ContactUrl))
        {
            document.Info.Contact = new OpenApiContact
            {
                Name = opts.ContactName,
                Email = opts.ContactEmail,
                Url = !string.IsNullOrWhiteSpace(opts.ContactUrl)
                    ? new Uri(opts.ContactUrl) : null
            };
        }

        // ── License ──────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(opts.LicenseName))
        {
            document.Info.License = new OpenApiLicense
            {
                Name = opts.LicenseName,
                Url = !string.IsNullOrWhiteSpace(opts.LicenseUrl)
                    ? new Uri(opts.LicenseUrl) : null
            };
        }

        // ── Servers ──────────────────────────────────────────────────
        if (opts.Servers is { Count: > 0 })
        {
            document.Servers = opts.Servers
                .Select(s => new OpenApiServer
                {
                    Url = s.Url,
                    Description = s.Description
                })
                .ToList();
        }

        // ── External docs ────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(opts.ExternalDocsUrl))
        {
            document.ExternalDocs = new OpenApiExternalDocs
            {
                Url = new Uri(opts.ExternalDocsUrl!),
                Description = opts.ExternalDocsDescription ?? "External Documentation"
            };
        }

        // ── Custom metadata extensions ───────────────────────────────
        if (opts.Metadata is { Count: > 0 })
        {
            document.Info.Extensions ??= new Dictionary<string, IOpenApiExtension>();
            foreach (var (key, value) in opts.Metadata)
            {
                document.Info.Extensions[$"x-{key}"] =
                    new JsonNodeExtension(JsonValue.Create(value));
            }
        }

        return Task.CompletedTask;
    }
}

// ─────────────────────────────────────────────────────────────────────
// 2. Injects security schemes + global security requirements
// ─────────────────────────────────────────────────────────────────────

/// <summary>
/// Transforms the OpenAPI document to inject security schemes (JWT Bearer,
/// API Key, OAuth2, OpenID Connect) and their global security requirements
/// from <see cref="LhaSwaggerOptions.SecuritySchemes"/>.
/// </summary>
internal sealed class LhaDocumentSecurityTransformer(
    IOptions<LhaSwaggerOptions> options) : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = options.Value.SecuritySchemes;
        if (schemes is not { Count: > 0 })
            return Task.CompletedTask;

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        foreach (var cfg in schemes)
        {
            var scheme = BuildSecurityScheme(cfg);
            document.Components.SecuritySchemes[cfg.Name] = scheme;

            // Add global security requirement using a typed reference
            document.Security ??= [];
            var schemeRef = new OpenApiSecuritySchemeReference(cfg.Name, document);

            document.Security.Add(new OpenApiSecurityRequirement
            {
                [schemeRef] = GetScopes(cfg)
            });
        }

        return Task.CompletedTask;
    }

    private static OpenApiSecurityScheme BuildSecurityScheme(SwaggerSecurityScheme cfg)
    {
        var type = cfg.Type?.ToLowerInvariant();

        return type switch
        {
            "http" => new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = cfg.Scheme ?? "bearer",
                BearerFormat = cfg.BearerFormat ?? "JWT",
                Description = cfg.Description ?? "Enter your JWT token.",
                In = ParseLocation(cfg.Location)
            },

            "apikey" => new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = cfg.ParameterName ?? cfg.Name,
                In = ParseLocation(cfg.Location),
                Description = cfg.Description ?? "Enter your API key."
            },

            "oauth2" => BuildOAuth2Scheme(cfg),

            "openidconnect" => new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OpenIdConnect,
                OpenIdConnectUrl = !string.IsNullOrWhiteSpace(cfg.OpenIdConnectUrl)
                    ? new Uri(cfg.OpenIdConnectUrl) : null,
                Description = cfg.Description ?? "OpenID Connect authentication."
            },

            _ => new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = cfg.Scheme ?? "bearer",
                BearerFormat = cfg.BearerFormat,
                Description = cfg.Description
            }
        };
    }

    private static OpenApiSecurityScheme BuildOAuth2Scheme(SwaggerSecurityScheme cfg)
    {
        var scopes = cfg.Scopes ?? [];

        var flow = new OpenApiOAuthFlow
        {
            Scopes = scopes
        };

        if (!string.IsNullOrWhiteSpace(cfg.AuthorizationUrl))
            flow.AuthorizationUrl = new Uri(cfg.AuthorizationUrl);
        if (!string.IsNullOrWhiteSpace(cfg.TokenUrl))
            flow.TokenUrl = new Uri(cfg.TokenUrl);
        if (!string.IsNullOrWhiteSpace(cfg.RefreshUrl))
            flow.RefreshUrl = new Uri(cfg.RefreshUrl);

        // Determine which OAuth2 flow based on available URLs
        var flows = new OpenApiOAuthFlows();

        if (!string.IsNullOrWhiteSpace(cfg.AuthorizationUrl) && !string.IsNullOrWhiteSpace(cfg.TokenUrl))
            flows.AuthorizationCode = flow;
        else if (!string.IsNullOrWhiteSpace(cfg.TokenUrl))
            flows.ClientCredentials = flow;
        else if (!string.IsNullOrWhiteSpace(cfg.AuthorizationUrl))
            flows.Implicit = flow;

        return new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = flows,
            Description = cfg.Description ?? "OAuth2 authentication."
        };
    }

    private static ParameterLocation ParseLocation(string? location)
        => location?.ToLowerInvariant() switch
        {
            "query" => ParameterLocation.Query,
            "cookie" => ParameterLocation.Cookie,
            _ => ParameterLocation.Header
        };

    private static List<string> GetScopes(SwaggerSecurityScheme cfg)
        => cfg.Scopes?.Keys.ToList() ?? [];
}

// ─────────────────────────────────────────────────────────────────────
// 3. Marks endpoints requiring [Authorize] with security locks
// ─────────────────────────────────────────────────────────────────────

/// <summary>
/// An operation transformer that adds security requirements to individual
/// operations based on their authorization metadata (e.g. <c>[Authorize]</c>,
/// <c>RequireAuthorization()</c>).
/// </summary>
internal sealed class LhaOperationSecurityTransformer(
    IOptions<LhaSwaggerOptions> options) : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemes = options.Value.SecuritySchemes;
        if (schemes is not { Count: > 0 })
            return Task.CompletedTask;

        // Check if the endpoint requires authorization
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        var hasAuth = metadata.Any(m =>
            m.GetType().Name is "AuthorizeAttribute" or "AuthorizeFilter");

        // Also check for IAuthorizeData on minimal APIs
        hasAuth = hasAuth || metadata.Any(m =>
            m.GetType().GetInterfaces().Any(i => i.Name == "IAuthorizeData"));

        var allowAnonymous = metadata.Any(m =>
            m.GetType().Name is "AllowAnonymousAttribute" or "AllowAnonymousFilter");

        if (!hasAuth || allowAnonymous)
            return Task.CompletedTask;

        operation.Security ??= [];
        foreach (var scheme in schemes)
        {
            var schemeRef = new OpenApiSecuritySchemeReference(scheme.Name, null!);

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [schemeRef] = scheme.Scopes?.Keys.ToList() ?? []
            });
        }

        // Add 401/403 responses if not present
        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        return Task.CompletedTask;
    }
}
