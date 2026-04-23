using System.Linq.Expressions;
using System.Reflection;
using LHA.MultiTenancy;
using LHA.Core.Users;

namespace LHA.Auditing;

/// <summary>
/// Default <see cref="IAuditPropertySetter"/> that uses <see cref="TimeProvider"/>
/// for timestamps and <see cref="IAuditUserProvider"/> for user identity.
/// <para>
/// Respects multi-tenant boundaries: creator/modifier IDs are only set when
/// the entity's tenant matches the current user's tenant.
/// </para>
/// </summary>
public sealed class AuditPropertySetter : IAuditPropertySetter
{
    private readonly IAuditUserProvider _userProvider;
    private readonly ICurrentTenant _currentTenant;
    private readonly TimeProvider _timeProvider;

    public AuditPropertySetter(
        IAuditUserProvider userProvider,
        ICurrentTenant currentTenant,
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(userProvider);
        ArgumentNullException.ThrowIfNull(currentTenant);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _userProvider = userProvider;
        _currentTenant = currentTenant;
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public void SetCreationProperties(object targetObject)
    {
        SetCreationTime(targetObject);
        SetCreatorId(targetObject);
    }

    /// <inheritdoc />
    public void SetModificationProperties(object targetObject)
    {
        SetLastModificationTime(targetObject);
        SetLastModifierId(targetObject);
    }

    /// <inheritdoc />
    public void SetDeletionProperties(object targetObject)
    {
        SetIsDeleted(targetObject);
        SetDeletionTime(targetObject);
        SetDeleterId(targetObject);
    }

    /// <inheritdoc />
    public void IncrementEntityVersionProperty(object targetObject)
    {
        if (targetObject is IHasEntityVersion)
        {
            TrySetProperty<IHasEntityVersion, int>(
                targetObject, e => e.EntityVersion, e => e.EntityVersion + 1);
        }
    }

    private void SetCreationTime(object targetObject)
    {
        if (targetObject is IHasCreationTime obj && obj.CreationTime == default)
        {
            TrySetProperty<IHasCreationTime, DateTimeOffset>(
                targetObject, e => e.CreationTime, _ => _timeProvider.GetUtcNow().UtcDateTime);
        }
    }

    private void SetCreatorId(object targetObject)
    {
        var userId = _userProvider.UserId ?? CurrentUserDefaults.SystemUserId;
        if (!IsSameTenant(targetObject, userId)) return;

        if (targetObject is IMayHaveCreator may && !may.CreatorId.HasValue)
        {
            TrySetProperty<IMayHaveCreator, Guid?>(
                targetObject, e => e.CreatorId, _ => userId);
        }
        else if (targetObject is IMustHaveCreator must && must.CreatorId == default)
        {
            TrySetProperty<IMustHaveCreator, Guid>(
                targetObject, e => e.CreatorId, _ => userId);
        }
    }

    private void SetLastModificationTime(object targetObject)
    {
        if (targetObject is IHasModificationTime)
        {
            TrySetProperty<IHasModificationTime, DateTimeOffset?>(
                targetObject, e => e.LastModificationTime, _ => _timeProvider.GetUtcNow().UtcDateTime);
        }
    }

    private void SetLastModifierId(object targetObject)
    {
        if (targetObject is not IModificationAuditedObject) return;

        var userId = _userProvider.UserId ?? CurrentUserDefaults.SystemUserId;

        if (!IsSameTenant(targetObject, userId))
        {
            TrySetProperty<IModificationAuditedObject, Guid?>(
                targetObject, e => e.LastModifierId, _ => null);
            return;
        }

        TrySetProperty<IModificationAuditedObject, Guid?>(
            targetObject, e => e.LastModifierId, _ => userId);
    }

    private void SetIsDeleted(object targetObject)
    {
        if (targetObject is ISoftDelete)
        {
            TrySetProperty<ISoftDelete, bool>(targetObject, e => e.IsDeleted, _ => true);
        }
    }

    private void SetDeletionTime(object targetObject)
    {
        if (targetObject is IHasDeletionTime obj && obj.DeletionTime is null)
        {
            TrySetProperty<IHasDeletionTime, DateTimeOffset?>(
                targetObject, e => e.DeletionTime, _ => _timeProvider.GetUtcNow().UtcDateTime);
        }
    }

    private void SetDeleterId(object targetObject)
    {
        if (targetObject is not IDeletionAuditedObject) return;

        var userId = _userProvider.UserId ?? CurrentUserDefaults.SystemUserId;

        if (!IsSameTenant(targetObject, userId))
        {
            TrySetProperty<IDeletionAuditedObject, Guid?>(
                targetObject, e => e.DeleterId, _ => null);
            return;
        }

        TrySetProperty<IDeletionAuditedObject, Guid?>(
            targetObject, e => e.DeleterId, _ => userId);
    }

    private bool IsSameTenant(object targetObject, Guid userId)
    {
        if (CurrentUserDefaults.IsSystemIdentity(userId))
        {
            return true; // System/Anonymous can operate across tenants
        }

        if (targetObject is IMultiTenant multiTenant)
        {
            return multiTenant.TenantId == _userProvider.TenantId;
        }

        return true; // Non-tenant entity — always allow
    }

    /// <summary>
    /// Attempts to set a property via reflection.
    /// When the property expression targets an interface property (which may lack a setter),
    /// falls back to looking up the property on the concrete type.
    /// Silently fails if no setter exists at all (truly read-only / write-once patterns).
    /// </summary>
    private static void TrySetProperty<TInterface, TValue>(
        object target,
        Expression<Func<TInterface, TValue>> propertyExpression,
        Func<TInterface, TValue> valueFactory)
    {
        if (target is not TInterface typed) return;

        if (propertyExpression.Body is not MemberExpression memberExpr) return;
        if (memberExpr.Member is not PropertyInfo prop) return;

        var setter = prop.GetSetMethod(nonPublic: true);

        // Interface properties typically only declare a getter.
        // Look up the same-named property on the concrete type to find the setter.
        if (setter is null)
        {
            var concreteProp = target.GetType().GetProperty(
                prop.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            setter = concreteProp?.GetSetMethod(nonPublic: true);
        }

        if (setter is null) return;

        setter.Invoke(target, [valueFactory(typed)]);
    }
}
