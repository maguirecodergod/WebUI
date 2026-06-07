namespace LHA.BlazorWasm.Shared.Models;

/// <summary>
/// General status enum used across all entities to indicate lifecycle state.
/// </summary>
public enum CGeneralStatus
{
    /// <summary>
    /// 0 - Success
    /// </summary>
    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-circle")]
    Success,

    /// <summary>
    /// 1 - Error
    /// </summary>
    [StatusBadge(CBadgeStyle.Danger, Variant = CBadgeVariant.Soft, Icon = "bi bi-exclamation-circle")]
    Error,

    /// <summary>
    /// 2 - Info
    /// </summary>
    [StatusBadge(CBadgeStyle.Info, Variant = CBadgeVariant.Soft, Icon = "bi bi-info-circle")]
    Info,

    /// <summary>
    /// 3 - Warning
    /// </summary>
    [StatusBadge(CBadgeStyle.Warning, Variant = CBadgeVariant.Soft, Icon = "bi bi-exclamation-triangle")]
    Warning
}