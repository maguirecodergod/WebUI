using LHA.Auditing;
using LHA.Ddd.Domain;
using LHA.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="EntityTypeBuilder"/> for configuring
/// audit, concurrency, soft-delete, and multi-tenancy columns by convention.
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Maximum length for the <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> column.
    /// </summary>
    public const int ConcurrencyStampMaxLength = 40;

    /// <summary>
    /// Applies all convention-based configurations for auditing, soft-delete,
    /// concurrency, and multi-tenancy properties if the entity type implements the
    /// corresponding interfaces.
    /// </summary>
    public static void ConfigureByConvention(this EntityTypeBuilder builder)
    {
        builder.TryConfigureConcurrencyStamp();
        builder.TryConfigureSoftDelete();
        builder.TryConfigureDeletionTime();
        builder.TryConfigureDeletionAudited();
        builder.TryConfigureMayHaveCreator();
        builder.TryConfigureMustHaveCreator();
        builder.TryConfigureCreationTime();
        builder.TryConfigureLastModificationTime();
        builder.TryConfigureModificationAudited();
        builder.TryConfigureMultiTenant();
        builder.TryConfigureEntityVersion();
    }

    // ─── Concurrency ─────────────────────────────────────────────────

    /// <summary>
    /// Configures <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> as a concurrency token.
    /// </summary>
    public static void TryConfigureConcurrencyStamp(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IHasConcurrencyStamp)))
            return;

        builder.Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp))
            .IsConcurrencyToken()
            .HasMaxLength(ConcurrencyStampMaxLength)
            .HasColumnName(nameof(IHasConcurrencyStamp.ConcurrencyStamp));
    }

    // ─── Soft Delete ─────────────────────────────────────────────────

    /// <summary>
    /// Configures <see cref="ISoftDelete.IsDeleted"/> with a default value of <c>false</c>.
    /// </summary>
    public static void TryConfigureSoftDelete(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(ISoftDelete)))
            return;

        builder.Property(nameof(ISoftDelete.IsDeleted))
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName(nameof(ISoftDelete.IsDeleted));
    }

    /// <summary>
    /// Configures <see cref="IHasDeletionTime.DeletionTime"/> column.
    /// Also applies soft-delete configuration.
    /// </summary>
    public static void TryConfigureDeletionTime(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IHasDeletionTime)))
            return;

        builder.TryConfigureSoftDelete();

        builder.Property(nameof(IHasDeletionTime.DeletionTime))
            .IsRequired(false)
            .HasColumnName(nameof(IHasDeletionTime.DeletionTime));
    }

    /// <summary>
    /// Configures <see cref="IDeletionAuditedObject.DeleterId"/> column.
    /// Also applies deletion time configuration.
    /// </summary>
    public static void TryConfigureDeletionAudited(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IDeletionAuditedObject)))
            return;

        builder.TryConfigureDeletionTime();

        builder.Property(nameof(IDeletionAuditedObject.DeleterId))
            .IsRequired(false)
            .HasColumnName(nameof(IDeletionAuditedObject.DeleterId));
    }

    // ─── Creator ─────────────────────────────────────────────────────

    /// <summary>
    /// Configures optional <see cref="IMayHaveCreator.CreatorId"/> column.
    /// </summary>
    public static void TryConfigureMayHaveCreator(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IMayHaveCreator)))
            return;

        builder.Property(nameof(IMayHaveCreator.CreatorId))
            .IsRequired(false)
            .HasColumnName(nameof(IMayHaveCreator.CreatorId));
    }

    /// <summary>
    /// Configures required <see cref="IMustHaveCreator.CreatorId"/> column.
    /// </summary>
    public static void TryConfigureMustHaveCreator(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IMustHaveCreator)))
            return;

        builder.Property(nameof(IMustHaveCreator.CreatorId))
            .IsRequired()
            .HasColumnName(nameof(IMustHaveCreator.CreatorId));
    }

    // ─── Creation / Modification Time ────────────────────────────────

    /// <summary>
    /// Configures <see cref="IHasCreationTime.CreationTime"/> column.
    /// </summary>
    public static void TryConfigureCreationTime(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IHasCreationTime)))
            return;

        builder.Property(nameof(IHasCreationTime.CreationTime))
            .IsRequired()
            .HasColumnName(nameof(IHasCreationTime.CreationTime));
    }

    /// <summary>
    /// Configures <see cref="IHasModificationTime.LastModificationTime"/> column.
    /// </summary>
    public static void TryConfigureLastModificationTime(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IHasModificationTime)))
            return;

        builder.Property(nameof(IHasModificationTime.LastModificationTime))
            .IsRequired(false)
            .HasColumnName(nameof(IHasModificationTime.LastModificationTime));
    }

    /// <summary>
    /// Configures <see cref="IModificationAuditedObject.LastModifierId"/> column.
    /// Also applies modification time configuration.
    /// </summary>
    public static void TryConfigureModificationAudited(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IModificationAuditedObject)))
            return;

        builder.TryConfigureLastModificationTime();

        builder.Property(nameof(IModificationAuditedObject.LastModifierId))
            .IsRequired(false)
            .HasColumnName(nameof(IModificationAuditedObject.LastModifierId));
    }

    // ─── Multi-Tenancy ───────────────────────────────────────────────

    /// <summary>
    /// Configures <see cref="IMultiTenant.TenantId"/> column.
    /// </summary>
    public static void TryConfigureMultiTenant(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IMultiTenant)))
            return;

        builder.Property(nameof(IMultiTenant.TenantId))
            .IsRequired(false)
            .HasColumnName(nameof(IMultiTenant.TenantId));
    }

    // ─── Entity Version ──────────────────────────────────────────────

    /// <summary>
    /// Configures <see cref="IHasEntityVersion.EntityVersion"/> column.
    /// </summary>
    public static void TryConfigureEntityVersion(this EntityTypeBuilder builder)
    {
        if (!builder.Metadata.ClrType.IsAssignableTo(typeof(IHasEntityVersion)))
            return;

        builder.Property(nameof(IHasEntityVersion.EntityVersion))
            .IsRequired()
            .HasDefaultValue(0)
            .HasColumnName(nameof(IHasEntityVersion.EntityVersion));
    }

    // ─── Composite helpers ───────────────────────────────────────────

    /// <summary>
    /// Configures creation audit columns (creation time + creator).
    /// </summary>
    public static void ConfigureCreationAudited(this EntityTypeBuilder builder)
    {
        builder.TryConfigureCreationTime();
        builder.TryConfigureMayHaveCreator();
    }

    /// <summary>
    /// Configures full audit columns (creation + modification + deletion).
    /// </summary>
    public static void ConfigureFullAudited(this EntityTypeBuilder builder)
    {
        builder.TryConfigureCreationTime();
        builder.TryConfigureMayHaveCreator();
        builder.TryConfigureLastModificationTime();
        builder.TryConfigureModificationAudited();
        builder.TryConfigureDeletionAudited();
    }
}
