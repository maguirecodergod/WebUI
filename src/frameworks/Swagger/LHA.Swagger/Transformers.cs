using System.Text.Json.Nodes;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
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

// ─────────────────────────────────────────────────────────────────────
// 4. Enriches schemas with XML comments and enum names
// ─────────────────────────────────────────────────────────────────────

/// <summary>
/// Adds XML documentation comments to generated schemas and exposes enum names
/// even when enum members do not have XML comments.
/// </summary>
internal sealed partial class LhaSchemaDocumentationTransformer : IOpenApiSchemaTransformer
{
    internal static readonly object CacheLock = new();
    internal static readonly Dictionary<Assembly, Dictionary<string, string>> XmlDocsByAssembly = [];

    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        ApplyTypeSummary(schema, context);
        ApplyPropertySummary(schema, context);
        ApplyParameterSummary(schema, context);
        ApplyEnumDocumentation(schema, context);

        return Task.CompletedTask;
    }

    private static void ApplyTypeSummary(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context)
    {
        if (!string.IsNullOrWhiteSpace(schema.Description))
            return;

        var type = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
        if (TryGetXmlSummary(type, out var summary))
        {
            schema.Description = summary;
        }
    }

    private static void ApplyPropertySummary(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context)
    {
        if (!string.IsNullOrWhiteSpace(schema.Description))
            return;

        PropertyInfo? property = context.JsonPropertyInfo?.AttributeProvider as PropertyInfo;

        // Fallback: resolve via reflection when AttributeProvider is null
        // (source-generated JSON or certain .NET 10+ scenarios)
        if (property is null && context.JsonPropertyInfo is not null)
        {
            var declaringType = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type)
                                ?? context.JsonTypeInfo.Type;
            property = declaringType.GetProperty(context.JsonPropertyInfo.Name);
        }

        if (property is null)
            return;

        if (TryGetXmlSummary(property, out var summary))
        {
            schema.Description = summary;
        }
    }

    private static void ApplyParameterSummary(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context)
    {
        if (!string.IsNullOrWhiteSpace(schema.Description))
            return;

        var description = context.ParameterDescription?.ModelMetadata?.Description;
        if (!string.IsNullOrWhiteSpace(description))
        {
            schema.Description = NormalizeXmlText(description);
        }
    }

    private static void ApplyEnumDocumentation(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context)
    {
        var type = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;
        if (!type.IsEnum)
            return;

        var names = Enum.GetNames(type);
        if (names.Length == 0)
            return;

        // Resolve underlying integer values for each member
        var underlyingValues = names
            .Select(name => Convert.ToInt64(type.GetField(name)!.GetValue(null)))
            .ToArray();

        // Use XML summary if available; otherwise fall back to "Name = numericValue"
        var descriptions = new string[names.Length];
        for (var i = 0; i < names.Length; i++)
        {
            descriptions[i] = TryGetXmlSummary(type.GetField(names[i])!, out var summary)
                ? summary
                : $"{underlyingValues[i]}";
        }

        schema.Extensions ??= new Dictionary<string, IOpenApiExtension>();
        schema.Extensions["x-enumNames"] = new JsonNodeExtension(CreateStringArray(names));
        schema.Extensions["x-enumDescriptions"] = new JsonNodeExtension(CreateStringArray(descriptions));

        var enumDescription = BuildEnumDescription(names, descriptions);
        schema.Description = string.IsNullOrWhiteSpace(schema.Description)
            ? enumDescription
            : $"{schema.Description}{Environment.NewLine}{Environment.NewLine}{enumDescription}";
    }

    private static string BuildEnumDescription(string[] names, string[] descriptions)
    {
        var lines = new List<string> { "Allowed values:" };

        for (var i = 0; i < names.Length; i++)
        {
            lines.Add($"- `{names[i]}`: {descriptions[i]}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static JsonArray CreateStringArray(IEnumerable<string> values)
    {
        var array = new JsonArray();

        foreach (var value in values)
        {
            array.Add(JsonValue.Create(value));
        }

        return array;
    }

    internal static bool TryGetXmlSummary(MemberInfo member, out string summary)
    {
        var docs = GetXmlDocs(member.Module.Assembly);
        var memberName = GetXmlMemberName(member);

        return docs.TryGetValue(memberName, out summary!);
    }

    internal static Dictionary<string, string> GetXmlDocs(Assembly assembly)
    {
        lock (CacheLock)
        {
            if (XmlDocsByAssembly.TryGetValue(assembly, out var cached))
                return cached;

            var docs = LoadXmlDocs(assembly);
            XmlDocsByAssembly[assembly] = docs;
            return docs;
        }
    }

    private static Dictionary<string, string> LoadXmlDocs(Assembly assembly)
    {
        var xmlPath = Path.ChangeExtension(assembly.Location, ".xml");
        if (string.IsNullOrWhiteSpace(xmlPath) || !File.Exists(xmlPath))
            return [];

        var document = XDocument.Load(xmlPath);

        return document.Descendants("member")
            .Select(member => new
            {
                Name = member.Attribute("name")?.Value,
                Summary = NormalizeXmlText(member.Element("summary")?.Value)
            })
            .Where(member => !string.IsNullOrWhiteSpace(member.Name) &&
                             !string.IsNullOrWhiteSpace(member.Summary))
            .ToDictionary(member => member.Name!, member => member.Summary!);
    }

    internal static string GetXmlMemberName(MemberInfo member)
        => member switch
        {
            Type type => $"T:{GetXmlTypeName(type)}",
            PropertyInfo property => $"P:{GetXmlTypeName(property.DeclaringType!)}.{property.Name}",
            FieldInfo field => $"F:{GetXmlTypeName(field.DeclaringType!)}.{field.Name}",
            _ => member.Name
        };

    internal static string GetXmlTypeName(Type type)
    {
        if (!type.IsGenericType)
            return (type.FullName ?? type.Name).Replace('+', '.');

        // Preserve the generic arity (e.g. `1, `2) — the C# compiler includes it
        // in XML documentation member names: "ApiResponse`1" not "ApiResponse".
        var genericTypeName = type.GetGenericTypeDefinition().FullName ?? type.Name;
        return genericTypeName.Replace('+', '.');
    }

    internal static string? NormalizeXmlText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return WhitespaceRegex().Replace(value, " ").Trim();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}

// ─────────────────────────────────────────────────────────────────────
// 5. Enriches operation parameters with XML documentation comments
// ─────────────────────────────────────────────────────────────────────

/// <summary>
/// An operation transformer that enriches endpoint parameters (including those
/// from <c>[AsParameters]</c>) with XML documentation descriptions and sets
/// the operation summary from method-level XML comments when available.
/// </summary>
internal sealed class LhaOperationDocumentationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        EnrichParameterDescriptions(operation, context);
        EnrichRequestBodyDescription(operation, context);
        return Task.CompletedTask;
    }

    private static void EnrichParameterDescriptions(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context)
    {
        if (operation.Parameters is null or { Count: 0 })
            return;

        // Build lookup: parameter name → (ContainerType, PropertyName)
        // from ApiDescription which tracks [AsParameters] decomposition
        var paramLookup = new Dictionary<string, (Type? ContainerType, string? PropertyName)>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var paramDesc in context.Description.ParameterDescriptions)
        {
            var metadata = paramDesc.ModelMetadata;
            if (metadata?.ContainerType is not null
                && !string.IsNullOrWhiteSpace(metadata.PropertyName))
            {
                paramLookup[paramDesc.Name] = (metadata.ContainerType, metadata.PropertyName);
            }
        }

        foreach (var param in operation.Parameters)
        {
            if (!string.IsNullOrWhiteSpace(param.Description))
                continue;

            if (param.Name is null
                || !paramLookup.TryGetValue(param.Name, out var info)
                || info.ContainerType is null
                || info.PropertyName is null)
                continue;

            var property = info.ContainerType.GetProperty(info.PropertyName);
            if (property is not null
                && LhaSchemaDocumentationTransformer.TryGetXmlSummary(property, out var summary))
            {
                param.Description = summary;
            }
        }
    }

    private static void EnrichRequestBodyDescription(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context)
    {
        if (operation.RequestBody is null)
            return;

        // For body parameters, try to find the XML summary from the DTO type
        foreach (var paramDesc in context.Description.ParameterDescriptions)
        {
            if (paramDesc.Source is not null
                && !string.Equals(paramDesc.Source.DisplayName, "Body", StringComparison.OrdinalIgnoreCase))
                continue;

            var type = paramDesc.Type;
            if (type is null)
                continue;

            if (LhaSchemaDocumentationTransformer.TryGetXmlSummary(type, out var summary)
                && string.IsNullOrWhiteSpace(operation.RequestBody.Description))
            {
                operation.RequestBody.Description = summary;
            }
        }
    }
}
