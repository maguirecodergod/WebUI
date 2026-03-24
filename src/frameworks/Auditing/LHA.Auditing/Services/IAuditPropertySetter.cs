namespace LHA.Auditing;

/// <summary>
/// Sets audit properties (creation time, creator, modification time, etc.)
/// on entity objects during persistence operations.
/// </summary>
public interface IAuditPropertySetter
{
    /// <summary>Sets <see cref="IHasCreationTime.CreationTime"/> and <see cref="IMayHaveCreator.CreatorId"/>.</summary>
    void SetCreationProperties(object targetObject);

    /// <summary>Sets <see cref="IHasModificationTime.LastModificationTime"/> and <see cref="IModificationAuditedObject.LastModifierId"/>.</summary>
    void SetModificationProperties(object targetObject);

    /// <summary>Sets <see cref="IHasDeletionTime.DeletionTime"/>, <see cref="ISoftDelete.IsDeleted"/>, and <see cref="IDeletionAuditedObject.DeleterId"/>.</summary>
    void SetDeletionProperties(object targetObject);

    /// <summary>Increments <see cref="IHasEntityVersion.EntityVersion"/>.</summary>
    void IncrementEntityVersionProperty(object targetObject);
}
