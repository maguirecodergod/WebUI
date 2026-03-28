using LHA.Ddd.Domain;

namespace LHA.PermissionManagement.Domain.PermissionTemplates;

/// <summary>
/// Join entity — links a <see cref="PermissionTemplateEntity"/> to a <see cref="PermissionGroupEntity"/>.
/// </summary>
public sealed class PermissionTemplateItemEntity : Entity<Guid>
{
    public Guid PermissionTemplateId { get; private set; }
    public Guid PermissionGroupId { get; private set; }

    private PermissionTemplateItemEntity() { }

    internal PermissionTemplateItemEntity(Guid id, Guid permissionTemplateId, Guid permissionGroupId)
    {
        Id = id;
        PermissionTemplateId = permissionTemplateId;
        PermissionGroupId = permissionGroupId;
    }
}
