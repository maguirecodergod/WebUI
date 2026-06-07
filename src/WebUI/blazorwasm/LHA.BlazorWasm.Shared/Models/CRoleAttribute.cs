namespace LHA.BlazorWasm.Shared.Models;

public enum CRoleAttribute
{
    /// <summary>
    /// 0 - System
    /// </summary>
    [StatusBadge(CBadgeStyle.Purple, Variant = CBadgeVariant.Soft, Icon = "bi bi-shield-lock", Tooltip = "Roles.SystemRoleTooltip")]
    System,

    /// <summary>
    /// 1 - Default
    /// </summary>
    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-circle", Tooltip = "Roles.DefaultRoleTooltip")]
    Default,

    /// <summary>
    /// 2 - Public
    /// </summary>
    [StatusBadge(CBadgeStyle.Amber, Variant = CBadgeVariant.Soft, Icon = "bi bi-globe", Tooltip = "Roles.PublicRoleTooltip")]
    Public
}
