namespace LHA.Swagger;

/// <summary>
/// Centralized configuration options for the LHA Swagger/OpenAPI module.
/// Bind from <c>appsettings.json</c> section <c>"Swagger"</c>.
/// </summary>
public sealed class LhaSwaggerOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Swagger";

    // ─── General ─────────────────────────────────────────────────────

    /// <summary>Whether the OpenAPI documentation is enabled (default: <c>true</c>).</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Environments where Swagger UI / Scalar is available.
    /// If empty, available in all environments.
    /// Example: <c>["Development", "Staging"]</c>.
    /// </summary>
    public string[] AllowedEnvironments { get; set; } = ["Development", "Staging"];

    /// <summary>Which UI provider to use (default: <see cref="SwaggerUiProvider.Scalar"/>).</summary>
    public SwaggerUiProvider UiProvider { get; set; } = SwaggerUiProvider.Scalar;

    // ─── Document metadata ───────────────────────────────────────────

    /// <summary>Service / API title displayed in the docs.</summary>
    public string Title { get; set; } = "LHA API";

    /// <summary>Short description of the service.</summary>
    public string? Description { get; set; }

    /// <summary>API version string (e.g. <c>"v1"</c>, <c>"v2"</c>).</summary>
    public string Version { get; set; } = "v1";

    /// <summary>Contact name shown in the OpenAPI info.</summary>
    public string? ContactName { get; set; }

    /// <summary>Contact email shown in the OpenAPI info.</summary>
    public string? ContactEmail { get; set; }

    /// <summary>Contact URL shown in the OpenAPI info.</summary>
    public string? ContactUrl { get; set; }

    /// <summary>License name (e.g. <c>"MIT"</c>).</summary>
    public string? LicenseName { get; set; }

    /// <summary>License URL.</summary>
    public string? LicenseUrl { get; set; }

    /// <summary>Terms of Service URL.</summary>
    public string? TermsOfServiceUrl { get; set; }

    /// <summary>External documentation URL.</summary>
    public string? ExternalDocsUrl { get; set; }

    /// <summary>External documentation description.</summary>
    public string? ExternalDocsDescription { get; set; }

    // ─── Build metadata ──────────────────────────────────────────────

    /// <summary>
    /// Additional key-value metadata injected into the OpenAPI info extensions.
    /// Useful for build hash, deployment timestamp, environment name, etc.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = [];

    // ─── API versioning ──────────────────────────────────────────────

    /// <summary>
    /// Named OpenAPI documents. Each entry produces a separate <c>/openapi/{name}.json</c>.
    /// When empty, a single document named <see cref="Version"/> is generated.
    /// </summary>
    public List<ApiVersionDocument> Documents { get; set; } = [];

    // ─── Security ────────────────────────────────────────────────────

    /// <summary>Security schemes to register in the OpenAPI document.</summary>
    public List<SwaggerSecurityScheme> SecuritySchemes { get; set; } = [];

    // ─── Route customization ─────────────────────────────────────────

    /// <summary>Route prefix for OpenAPI JSON (default: <c>"openapi"</c>).</summary>
    public string OpenApiRoutePrefix { get; set; } = "openapi";

    /// <summary>Route prefix for Swagger UI (default: <c>"swagger"</c>).</summary>
    public string SwaggerUiRoutePrefix { get; set; } = "swagger";

    /// <summary>Route prefix for Scalar UI (default: <c>"scalar"</c>).</summary>
    public string ScalarRoutePrefix { get; set; } = "scalar";

    // ─── Server URLs ─────────────────────────────────────────────────

    /// <summary>
    /// Server URLs to include in the OpenAPI document.
    /// Useful for gateway / public-facing URLs.
    /// </summary>
    public List<SwaggerServer> Servers { get; set; } = [];
}

// ─── Sub-types ─────────────────────────────────────────────────────────

/// <summary>Defines a named API version document.</summary>
public sealed class ApiVersionDocument
{
    /// <summary>Document name used in the route (e.g. <c>"v1"</c>, <c>"v2"</c>).</summary>
    public required string Name { get; set; }

    /// <summary>Display title for this version.</summary>
    public string? Title { get; set; }

    /// <summary>Description for this version.</summary>
    public string? Description { get; set; }
}

/// <summary>Defines a security scheme for the OpenAPI document.</summary>
public sealed class SwaggerSecurityScheme
{
    /// <summary>Scheme identifier (e.g. <c>"Bearer"</c>, <c>"ApiKey"</c>).</summary>
    public required string Name { get; set; }

    /// <summary>
    /// Scheme type: <c>"Http"</c>, <c>"ApiKey"</c>, <c>"OAuth2"</c>, <c>"OpenIdConnect"</c>.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>Where the credential is sent: <c>"Header"</c>, <c>"Query"</c>, <c>"Cookie"</c>.</summary>
    public string Location { get; set; } = "Header";

    /// <summary>Header / query parameter name (for ApiKey schemes).</summary>
    public string? ParameterName { get; set; }

    /// <summary>HTTP scheme (e.g. <c>"bearer"</c> for JWT).</summary>
    public string? Scheme { get; set; }

    /// <summary>Bearer format hint (e.g. <c>"JWT"</c>).</summary>
    public string? BearerFormat { get; set; }

    /// <summary>Description shown in the docs.</summary>
    public string? Description { get; set; }

    /// <summary>OpenID Connect discovery URL.</summary>
    public string? OpenIdConnectUrl { get; set; }

    /// <summary>OAuth2 authorization URL.</summary>
    public string? AuthorizationUrl { get; set; }

    /// <summary>OAuth2 token URL.</summary>
    public string? TokenUrl { get; set; }

    /// <summary>OAuth2 refresh URL.</summary>
    public string? RefreshUrl { get; set; }

    /// <summary>OAuth2 scopes as <c>"scope": "description"</c>.</summary>
    public Dictionary<string, string>? Scopes { get; set; }
}

/// <summary>Server entry for the OpenAPI document.</summary>
public sealed class SwaggerServer
{
    /// <summary>Server URL (e.g. <c>"https://api.example.com"</c>).</summary>
    public required string Url { get; set; }

    /// <summary>Description of the server (e.g. <c>"Production"</c>).</summary>
    public string? Description { get; set; }
}

/// <summary>Available UI providers for interactive docs.</summary>
public enum SwaggerUiProvider
{
    /// <summary>Scalar — modern, fast, beautiful API reference.</summary>
    Scalar = 0,

    /// <summary>Swashbuckle Swagger UI — classic, widely adopted.</summary>
    SwaggerUi = 1,

    /// <summary>Both Scalar and Swagger UI simultaneously.</summary>
    Both = 2,

    /// <summary>No UI — only the JSON endpoint.</summary>
    None = 3
}
