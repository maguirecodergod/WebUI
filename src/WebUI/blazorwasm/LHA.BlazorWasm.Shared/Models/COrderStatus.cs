namespace LHA.BlazorWasm.Shared.Models;

public enum COrderStatus
{
    /// <summary>
    /// 0 - Pending
    /// </summary>
    [StatusBadge(CBadgeStyle.Warning, Variant = CBadgeVariant.Soft, Icon = "bi bi-clock")]
    Pending,

    /// <summary>
    /// 1 - Processing
    /// </summary>
    [StatusBadge(CBadgeStyle.Primary, Variant = CBadgeVariant.Soft, Icon = "bi bi-gear-fill", IsPulse = true)]
    Processing,

    /// <summary>
    /// 2 - Completed
    /// </summary>
    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-circle-fill")]
    Completed,

    /// <summary>
    /// 3 - Cancelled
    /// </summary>
    [StatusBadge(CBadgeStyle.Danger, Variant = CBadgeVariant.Soft, Icon = "bi bi-x-circle-fill")]
    Cancelled
}

/// <summary>
/// Demonstrating Convention-Based resolution. 
/// Even without attributes, "Paid" will be Success and "Failed" will be Danger.
/// </summary>
public enum CPaymentStatus
{
    /// <summary>
    /// 0 - Unpaid
    /// </summary>
    [StatusBadge(CBadgeStyle.Secondary, Variant = CBadgeVariant.Soft, Icon = "bi bi-cash", Tooltip = "Awaiting payment")]
    Unpaid,

    /// <summary>
    /// 1 - Paid
    /// </summary>
    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-currency-dollar", Tooltip = "Payment confirmed")]
    Paid,

    /// <summary>
    /// 2 - Failed
    /// </summary>
    [StatusBadge(CBadgeStyle.Danger, Variant = CBadgeVariant.Soft, Icon = "bi bi-exclamation-triangle-fill", Tooltip = "Transaction failed")]
    Failed,

    /// <summary>
    /// 3 - Refunded
    /// </summary>
    [StatusBadge(CBadgeStyle.Info, Variant = CBadgeVariant.Soft, Icon = "bi bi-arrow-counterclockwise", Tooltip = "Amount returned to customer")]
    Refunded
}

