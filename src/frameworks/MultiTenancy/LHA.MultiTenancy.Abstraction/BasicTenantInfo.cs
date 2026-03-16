namespace LHA.MultiTenancy;

/// <summary>
/// Lightweight tenant identification info stored in the ambient context.
/// </summary>
/// <param name="TenantId">The tenant ID, or <see langword="null"/> for the host.</param>
/// <param name="Name">Optional tenant name.</param>
public sealed record BasicTenantInfo(Guid? TenantId, string? Name = null);
