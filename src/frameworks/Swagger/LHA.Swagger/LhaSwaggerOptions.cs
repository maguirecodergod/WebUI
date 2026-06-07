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

    /// <summary>Route prefix for Redoc UI (default: <c>"redoc"</c>).</summary>
    public string RedocRoutePrefix { get; set; } = "redoc";

    // ─── Scalar customization ───────────────────────────────────────

    /// <summary>Scalar API reference customization.</summary>
    public LhaScalarOptions Scalar { get; set; } = new();

    // ─── Server URLs ─────────────────────────────────────────────────

    /// <summary>
    /// Server URLs to include in the OpenAPI document.
    /// Useful for gateway / public-facing URLs and Scalar deploy environment selector.
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

/// <summary>Scalar-specific UI options.</summary>
public sealed class LhaScalarOptions
{
    /// <summary>Controls Scalar developer tools visibility.</summary>
    public LhaScalarDeveloperToolsVisibility ShowDeveloperTools { get; set; } =
        LhaScalarDeveloperToolsVisibility.Always;

    /// <summary>Whether to show the Scalar API Client button.</summary>
    public bool ShowClientButton { get; set; } = true;

    /// <summary>
    /// Adds the current public request origin as a Scalar server, respecting reverse proxy headers.
    /// This makes Scalar use the deployed/ngrok host instead of localhost.
    /// </summary>
    public bool UseRequestOriginAsServer { get; set; } = true;

    /// <summary>Default HTTP clients used by Scalar's development tools / code samples.</summary>
    public List<ScalarHttpClientPreference> DefaultHttpClients { get; set; } =
    [
        new() { Target = "CSharp", Client = "HttpClient" },
        new() { Target = "JavaScript", Client = "Fetch" },
        new() { Target = "Shell", Client = "Curl" }
    ];

    /// <summary>MCP server displayed in Scalar.</summary>
    public ScalarMcpServerOptions McpServer { get; set; } = new();
}

/// <summary>Default HTTP client preference for a Scalar language target.</summary>
public sealed class ScalarHttpClientPreference
{
    /// <summary>Scalar target language, e.g. <c>"CSharp"</c>, <c>"JavaScript"</c>, <c>"Shell"</c>.</summary>
    public required string Target { get; set; }

    /// <summary>Scalar client, e.g. <c>"HttpClient"</c>, <c>"Fetch"</c>, <c>"Curl"</c>.</summary>
    public required string Client { get; set; }
}

/// <summary>Scalar MCP server integration options.</summary>
public sealed class ScalarMcpServerOptions
{
    /// <summary>Whether Scalar should expose MCP integration.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>Display name for the MCP server.</summary>
    public string? Name { get; set; }

    /// <summary>
    /// URL of the MCP server. If unset, the URL is resolved from the public request origin and <see cref="Path"/>.
    /// Relative URLs such as <c>"/mcp"</c> are also resolved against the public request origin.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>Relative MCP path used when <see cref="Url"/> is unset.</summary>
    public string Path { get; set; } = "/mcp";
}

/// <summary>Visibility policy for Scalar developer tools.</summary>
public enum LhaScalarDeveloperToolsVisibility
{
    /// <summary>Always show the developer tools toolbar, including MCP generation, in every allowed environment.</summary>
    Always = 0,

    /// <summary>Use Scalar's default localhost-only visibility.</summary>
    Localhost = 1,

    /// <summary>Hide the developer tools toolbar.</summary>
    Never = 2
}

/// <summary>Available UI providers for interactive docs.</summary>
public enum SwaggerUiProvider
{
    /// <summary>Scalar — modern, fast, beautiful API reference.</summary>
    Scalar = 0,

    /// <summary>Swashbuckle Swagger UI — classic, widely adopted.</summary>
    SwaggerUi = 1,

    /// <summary>Redoc — powerful OpenAPI/Swagger documentation renderer.</summary>
    Redoc = 2,

    /// <summary>Both Scalar and Swagger UI simultaneously.</summary>
    All = 3,

    /// <summary>Alias for <see cref="All"/>. Kept for configuration readability.</summary>
    Both = All,

    /// <summary>No UI — only the JSON endpoint.</summary>
    None = 4
}
