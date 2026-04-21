using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Shared.Models;

public enum CRoleAttribute
{
    [StatusBadge(CBadgeStyle.Purple, Variant = CBadgeVariant.Soft, Icon = "bi bi-shield-lock", Tooltip = "Roles.SystemRoleTooltip")]
    System,

    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-circle", Tooltip = "Roles.DefaultRoleTooltip")]
    Default,

    [StatusBadge(CBadgeStyle.Amber, Variant = CBadgeVariant.Soft, Icon = "bi bi-globe", Tooltip = "Roles.PublicRoleTooltip")]
    Public
}
