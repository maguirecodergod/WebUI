using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LHA.MultiTenancy;

/// <summary>
/// Reads <see cref="TenantConfiguration"/> from <see cref="IConfiguration"/>
/// under the <c>MultiTenancy:Tenants</c> section.
/// <para>
/// Example appsettings.json:
/// <code>
/// {
///   "MultiTenancy": {
///     "Tenants": [
///       {
///         "Id": "...",
///         "Name": "Acme",
///         "NormalizedName": "ACME",
///         "ConnectionStrings": { "Default": "Server=..." },
///         "IsActive": true,
///         "Region": { "Code": "EU" }
///       }
///     ]
///   }
/// }
/// </code>
/// </para>
/// </summary>
public sealed class ConfigurationTenantStore : ITenantStore
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationTenantStore> _logger;

    public ConfigurationTenantStore(
        IConfiguration configuration,
        ILogger<ConfigurationTenantStore>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        _configuration = configuration;
        _logger = logger ?? NullLogger<ConfigurationTenantStore>.Instance;
    }

    /// <inheritdoc />
    public Task<TenantConfiguration?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenants = LoadTenants();
        var tenant = tenants.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tenant);
    }

    /// <inheritdoc />
    public Task<TenantConfiguration?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedName);

        var tenants = LoadTenants();
        var tenant = tenants.FirstOrDefault(t =>
            string.Equals(t.NormalizedName, normalizedName, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(tenant);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<TenantConfiguration>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var tenants = LoadTenants();
        return Task.FromResult<IReadOnlyList<TenantConfiguration>>(tenants);
    }

    private List<TenantConfiguration> LoadTenants()
    {
        var section = _configuration.GetSection("MultiTenancy:Tenants");
        if (!section.Exists())
        {
            return [];
        }

        var tenants = new List<TenantConfiguration>();

        foreach (var child in section.GetChildren())
        {
            var idStr = child["Id"];
            if (!Guid.TryParse(idStr, out var id))
            {
                _logger.LogWarning("Skipping tenant entry with invalid Id: '{Id}'.", idStr);
                continue;
            }

            var name = child["Name"] ?? string.Empty;
            var tenant = new TenantConfiguration
            {
                Id = id,
                Name = name,
                NormalizedName = child["NormalizedName"] ?? name.ToUpperInvariant(),
                Status = bool.TryParse(child["IsActive"], out var active) && active
                    ? LHA.Core.CMasterStatus.Active
                    : LHA.Core.CMasterStatus.InActive
            };

            // Connection strings
            var connSection = child.GetSection("ConnectionStrings");
            if (connSection.Exists())
            {
                foreach (var connChild in connSection.GetChildren())
                {
                    if (connChild.Value is not null)
                    {
                        tenant.ConnectionStrings[connChild.Key] = connChild.Value;
                    }
                }
            }

            // Data residency region
            var regionCode = child["Region:Code"];
            if (!string.IsNullOrWhiteSpace(regionCode))
            {
                tenant.Region = new DataResidencyRegion { Code = regionCode, Description = child["Region:Description"] };
            }

            tenants.Add(tenant);
        }

        _logger.LogDebug("Loaded {Count} tenant(s) from configuration.", tenants.Count);
        return tenants;
    }
}
