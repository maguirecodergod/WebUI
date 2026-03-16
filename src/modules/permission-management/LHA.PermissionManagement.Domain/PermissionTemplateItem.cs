using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain;

/// <summary>
/// Join entity — links a <see cref="PermissionTemplate"/> to a <see cref="PermissionGroup"/>.
/// </summary>
public sealed class PermissionTemplateItem : Entity<Guid>
{
    public Guid PermissionTemplateId { get; private set; }
    public Guid PermissionGroupId { get; private set; }

    private PermissionTemplateItem() { }

    internal PermissionTemplateItem(Guid id, Guid permissionTemplateId, Guid permissionGroupId)
    {
        Id = id;
        PermissionTemplateId = permissionTemplateId;
        PermissionGroupId = permissionGroupId;
    }
}
