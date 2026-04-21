using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Shared.Models;

public enum CGeneralStatus
{
    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-circle")]
    Success,

    [StatusBadge(CBadgeStyle.Danger, Variant = CBadgeVariant.Soft, Icon = "bi bi-exclamation-circle")]
    Error,

    [StatusBadge(CBadgeStyle.Info, Variant = CBadgeVariant.Soft, Icon = "bi bi-info-circle")]
    Info,

    [StatusBadge(CBadgeStyle.Warning, Variant = CBadgeVariant.Soft, Icon = "bi bi-exclamation-triangle")]
    Warning
}

public enum CYesNo
{
    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-lg")]
    Yes,

    [StatusBadge(CBadgeStyle.Muted, Variant = CBadgeVariant.Soft, Icon = "bi bi-x-lg")]
    No
}

