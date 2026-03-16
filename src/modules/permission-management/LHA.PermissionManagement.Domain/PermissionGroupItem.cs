using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain;

/// <summary>
/// Join entity — links a <see cref="PermissionGroup"/> to a <see cref="PermissionDefinition"/>.
/// </summary>
public sealed class PermissionGroupItem : Entity<Guid>
{
    public Guid PermissionGroupId { get; private set; }
    public Guid PermissionDefinitionId { get; private set; }

    private PermissionGroupItem() { }

    internal PermissionGroupItem(Guid id, Guid permissionGroupId, Guid permissionDefinitionId)
    {
        Id = id;
        PermissionGroupId = permissionGroupId;
        PermissionDefinitionId = permissionDefinitionId;
    }
}
