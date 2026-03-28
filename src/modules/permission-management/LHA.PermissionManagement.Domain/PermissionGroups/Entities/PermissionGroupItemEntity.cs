using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionGroups;

/// <summary>
/// Join entity — links a <see cref="PermissionGroupEntity"/> to a <see cref="PermissionDefinitionEntity"/>.
/// </summary>
public sealed class PermissionGroupItemEntity : Entity<Guid>
{
    public Guid PermissionGroupId { get; private set; }
    public Guid PermissionDefinitionId { get; private set; }

    private PermissionGroupItemEntity() { }

    internal PermissionGroupItemEntity(Guid id, Guid permissionGroupId, Guid permissionDefinitionId)
    {
        Id = id;
        PermissionGroupId = permissionGroupId;
        PermissionDefinitionId = permissionDefinitionId;
    }
}
