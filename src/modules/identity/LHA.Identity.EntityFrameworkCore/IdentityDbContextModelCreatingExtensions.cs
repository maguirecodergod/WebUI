using LHA.Core;
using LHA.EntityFrameworkCore;
using LHA.Identity.Domain;
using LHA.Identity.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace LHA.Identity.EntityFrameworkCore;

/// <summary>
/// EF Core model configuration for all Identity module entities.
/// </summary>
public static class IdentityDbContextModelCreatingExtensions
{
    /// <summary>
    /// Configures all Identity entity types with tables, columns, indices, and relationships.
    /// </summary>
    public static void ConfigureIdentity(this ModelBuilder modelBuilder)
    {
        // Ignore the in-memory domain event wrapper
        modelBuilder.Ignore<LHA.Ddd.Domain.DomainEventRecord>();

        // ─── IdentityUser ────────────────────────────────────────────
        modelBuilder.Entity<IdentityUser>(b =>
        {
            b.ToTable("IdentityUsers");
            b.ConfigureByConvention();

            b.HasKey(u => u.Id);

            b.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(IdentityUserConsts.MaxUserNameLength);

            b.Property(u => u.NormalizedUserName)
                .IsRequired()
                .HasMaxLength(IdentityUserConsts.MaxUserNameLength);

            b.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(IdentityUserConsts.MaxEmailLength);

            b.Property(u => u.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(IdentityUserConsts.MaxEmailLength);

            b.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(IdentityUserConsts.MaxPasswordHashLength);

            b.Property(u => u.SecurityStamp)
                .IsRequired()
                .HasMaxLength(IdentityUserConsts.MaxSecurityStampLength);

            b.Property(u => u.PhoneNumber)
                .HasMaxLength(IdentityUserConsts.MaxPhoneNumberLength);

            b.Property(u => u.Status)
                .IsRequired()
                .HasDefaultValue(CMasterStatus.Active)
                .HasSentinel((CMasterStatus)0);

            b.Property(u => u.Name)
                .HasMaxLength(IdentityUserConsts.MaxNameLength);

            b.Property(u => u.Surname)
                .HasMaxLength(IdentityUserConsts.MaxSurnameLength);

            // Unique indexes
            b.HasIndex(u => u.NormalizedUserName);
            b.HasIndex(u => u.NormalizedEmail);

            // Navigations → backing fields
            b.HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(u => u.Roles)
                .HasField("_roles")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(u => u.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(u => u.Claims)
                .HasField("_claims")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(u => u.Logins)
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(u => u.Logins)
                .HasField("_logins")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            b.HasMany(u => u.Tokens)
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(u => u.Tokens)
                .HasField("_tokens")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // ─── IdentityUserRole ────────────────────────────────────────
        modelBuilder.Entity<IdentityUserRole>(b =>
        {
            b.ToTable("IdentityUserRoles");
            b.HasKey(ur => ur.Id);

            b.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
        });

        // ─── IdentityUserClaim ───────────────────────────────────────
        modelBuilder.Entity<IdentityUserClaim>(b =>
        {
            b.ToTable("IdentityUserClaims");
            b.HasKey(uc => uc.Id);

            b.Property(uc => uc.ClaimType)
                .IsRequired()
                .HasMaxLength(IdentityClaimConsts.MaxClaimTypeLength);

            b.Property(uc => uc.ClaimValue)
                .HasMaxLength(IdentityClaimConsts.MaxClaimValueLength);

            b.HasIndex(uc => uc.UserId);
        });

        // ─── IdentityUserLogin ───────────────────────────────────────
        modelBuilder.Entity<IdentityUserLogin>(b =>
        {
            b.ToTable("IdentityUserLogins");
            b.HasKey(ul => ul.Id);

            b.Property(ul => ul.LoginProvider)
                .IsRequired()
                .HasMaxLength(IdentityUserLoginConsts.MaxLoginProviderLength);

            b.Property(ul => ul.ProviderKey)
                .IsRequired()
                .HasMaxLength(IdentityUserLoginConsts.MaxProviderKeyLength);

            b.Property(ul => ul.ProviderDisplayName)
                .HasMaxLength(IdentityUserLoginConsts.MaxProviderDisplayNameLength);

            b.HasIndex(ul => new { ul.LoginProvider, ul.ProviderKey });
        });

        // ─── IdentityUserToken ───────────────────────────────────────
        modelBuilder.Entity<IdentityUserToken>(b =>
        {
            b.ToTable("IdentityUserTokens");
            b.HasKey(ut => ut.Id);

            b.Property(ut => ut.LoginProvider)
                .IsRequired()
                .HasMaxLength(IdentityUserTokenConsts.MaxLoginProviderLength);

            b.Property(ut => ut.Name)
                .IsRequired()
                .HasMaxLength(IdentityUserTokenConsts.MaxNameLength);

            b.Property(ut => ut.Value)
                .IsRequired()
                .HasMaxLength(IdentityUserTokenConsts.MaxValueLength);

            b.HasIndex(ut => new { ut.UserId, ut.LoginProvider, ut.Name }).IsUnique();
        });

        // ─── IdentityRole ────────────────────────────────────────────
        modelBuilder.Entity<IdentityRole>(b =>
        {
            b.ToTable("IdentityRoles");
            b.ConfigureByConvention();

            b.HasKey(r => r.Id);

            b.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(IdentityRoleConsts.MaxNameLength);

            b.Property(r => r.NormalizedName)
                .IsRequired()
                .HasMaxLength(IdentityRoleConsts.MaxNameLength);

            b.Property(r => r.Status)
                .IsRequired()
                .HasDefaultValue(CMasterStatus.Active)
                .HasSentinel((CMasterStatus)0);

            b.HasIndex(r => r.NormalizedName);

            b.HasMany(r => r.Claims)
                .WithOne()
                .HasForeignKey(rc => rc.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(r => r.Claims)
                .HasField("_claims")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        // ─── IdentityRoleClaim ───────────────────────────────────────
        modelBuilder.Entity<IdentityRoleClaim>(b =>
        {
            b.ToTable("IdentityRoleClaims");
            b.HasKey(rc => rc.Id);

            b.Property(rc => rc.ClaimType)
                .IsRequired()
                .HasMaxLength(IdentityClaimConsts.MaxClaimTypeLength);

            b.Property(rc => rc.ClaimValue)
                .HasMaxLength(IdentityClaimConsts.MaxClaimValueLength);

            b.HasIndex(rc => rc.RoleId);
        });

        // ─── IdentityClaimType ───────────────────────────────────────
        modelBuilder.Entity<IdentityClaimType>(b =>
        {
            b.ToTable("IdentityClaimTypes");
            b.ConfigureByConvention();

            b.HasKey(ct => ct.Id);

            b.Property(ct => ct.Name)
                .IsRequired()
                .HasMaxLength(IdentityClaimTypeConsts.MaxNameLength);

            b.Property(ct => ct.Regex)
                .HasMaxLength(IdentityClaimTypeConsts.MaxRegexLength);

            b.Property(ct => ct.RegexDescription)
                .HasMaxLength(IdentityClaimTypeConsts.MaxRegexDescriptionLength);

            b.Property(ct => ct.Description)
                .HasMaxLength(IdentityClaimTypeConsts.MaxDescriptionLength);

            b.HasIndex(ct => ct.Name).IsUnique();
        });

        // ─── IdentitySecurityLog ─────────────────────────────────────
        modelBuilder.Entity<IdentitySecurityLog>(b =>
        {
            b.ToTable("IdentitySecurityLogs");
            b.ConfigureByConvention();

            b.HasKey(sl => sl.Id);

            b.Property(sl => sl.ApplicationName)
                .HasMaxLength(IdentitySecurityLogConsts.MaxApplicationNameLength);

            b.Property(sl => sl.Identity)
                .HasMaxLength(IdentitySecurityLogConsts.MaxIdentityLength);

            b.Property(sl => sl.Action)
                .IsRequired()
                .HasMaxLength(IdentitySecurityLogConsts.MaxActionLength);

            b.Property(sl => sl.UserName)
                .HasMaxLength(IdentitySecurityLogConsts.MaxUserNameLength);

            b.Property(sl => sl.TenantName)
                .HasMaxLength(IdentitySecurityLogConsts.MaxTenantNameLength);

            b.Property(sl => sl.ClientId)
                .HasMaxLength(IdentitySecurityLogConsts.MaxClientIdLength);

            b.Property(sl => sl.CorrelationId)
                .HasMaxLength(IdentitySecurityLogConsts.MaxCorrelationIdLength);

            b.Property(sl => sl.ClientIpAddress)
                .HasMaxLength(IdentitySecurityLogConsts.MaxClientIpAddressLength);

            b.Property(sl => sl.BrowserInfo)
                .HasMaxLength(IdentitySecurityLogConsts.MaxBrowserInfoLength);

            b.Property(sl => sl.ExtraProperties)
                .HasMaxLength(IdentitySecurityLogConsts.MaxExtraPropertiesLength);

            b.HasIndex(sl => sl.UserId);
            b.HasIndex(sl => new { sl.TenantId, sl.Action });
            b.HasIndex(sl => sl.CreationTime);
        });

        // ─── IdentityPermissionGrant ─────────────────────────────────
        modelBuilder.Entity<IdentityPermissionGrant>(b =>
        {
            b.ToTable("IdentityPermissionGrants");

            b.HasKey(pg => pg.Id);

            b.Property(pg => pg.Name)
                .IsRequired()
                .HasMaxLength(IdentityPermissionGrantConsts.MaxNameLength);

            b.Property(pg => pg.ProviderName)
                .IsRequired()
                .HasMaxLength(IdentityPermissionGrantConsts.MaxProviderNameLength);

            b.Property(pg => pg.ProviderKey)
                .IsRequired()
                .HasMaxLength(IdentityPermissionGrantConsts.MaxProviderKeyLength);

            b.Property(pg => pg.IsGranted)
                .IsRequired()
                .HasDefaultValue(true);

            b.HasIndex(pg => new { pg.TenantId, pg.Name, pg.ProviderName, pg.ProviderKey })
                .IsUnique();
        });
    }
}
