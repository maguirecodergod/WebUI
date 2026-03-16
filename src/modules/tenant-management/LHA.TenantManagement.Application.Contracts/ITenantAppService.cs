using LHA.Ddd.Application;

namespace LHA.TenantManagement.Application.Contracts;

/// <summary>
/// Application service contract for the Tenant Management module.
/// Extends CRUD with tenant-specific operations: connection string management,
/// activation/deactivation, and name-based lookup.
/// </summary>
public interface ITenantAppService
    : ICrudAppService<TenantDto, Guid, GetTenantsInput, CreateTenantInput, UpdateTenantInput>
{
    /// <summary>
    /// Finds a tenant by display name (case-insensitive).
    /// Returns <c>null</c> if not found.
    /// </summary>
    Task<TenantDto?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all connection strings for the specified tenant.
    /// </summary>
    Task<List<TenantConnectionStringDto>> GetConnectionStringsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a named connection string on the tenant.
    /// </summary>
    Task<TenantDto> SetConnectionStringAsync(
        Guid tenantId,
        string name,
        string value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a named connection string from the tenant.
    /// </summary>
    Task<TenantDto> RemoveConnectionStringAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a tenant. No-op if already active.
    /// </summary>
    Task<TenantDto> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a tenant. No-op if already inactive.
    /// </summary>
    Task<TenantDto> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
