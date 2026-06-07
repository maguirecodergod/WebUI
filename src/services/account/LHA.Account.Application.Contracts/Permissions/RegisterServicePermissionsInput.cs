namespace LHA.Account.Application.Contracts.Permissions;

public sealed class RegisterServicePermissionsInput
{
    public required string ServiceName { get; init; }
    public required List<PermissionDefinitionInput> Permissions { get; init; }
    public required List<PermissionGroupInput> Groups { get; init; }
    public bool GrantAllToAdminRole { get; init; } = true;
}

public sealed class PermissionDefinitionInput
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string GroupName { get; init; }
    public PermissionRegistrationMultiTenancySide MultiTenancySide { get; init; } =
        PermissionRegistrationMultiTenancySide.Both;
}

public sealed class PermissionGroupInput
{
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
}

public enum PermissionRegistrationMultiTenancySide
{
    /// <summary>
    /// 0 - Both
    /// </summary>
    Both = 0,
    /// <summary>
    /// 1 - Host
    /// </summary>
    Host = 1,
    /// <summary>
    /// 2 - Tenant
    /// </summary>
    Tenant = 2,
}
