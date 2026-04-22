using LHA;

namespace LHA.BlazorWasm.Shared.Models;

public enum COrderStatus
{
    [StatusBadge(CBadgeStyle.Warning, Variant = CBadgeVariant.Soft, Icon = "bi bi-clock")]
    Pending,

    [StatusBadge(CBadgeStyle.Primary, Variant = CBadgeVariant.Soft, Icon = "bi bi-gear-fill", IsPulse = true)]
    Processing,

    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-check-circle-fill")]
    Completed,

    [StatusBadge(CBadgeStyle.Danger, Variant = CBadgeVariant.Soft, Icon = "bi bi-x-circle-fill")]
    Cancelled
}

/// <summary>
/// Demonstrating Convention-Based resolution. 
/// Even without attributes, "Paid" will be Success and "Failed" will be Danger.
/// </summary>
public enum CPaymentStatus
{
    [StatusBadge(CBadgeStyle.Secondary, Variant = CBadgeVariant.Soft, Icon = "bi bi-cash", Tooltip = "Awaiting payment")]
    Unpaid,

    [StatusBadge(CBadgeStyle.Success, Variant = CBadgeVariant.Soft, Icon = "bi bi-currency-dollar", Tooltip = "Payment confirmed")]
    Paid,

    [StatusBadge(CBadgeStyle.Danger, Variant = CBadgeVariant.Soft, Icon = "bi bi-exclamation-triangle-fill", Tooltip = "Transaction failed")]
    Failed,

    [StatusBadge(CBadgeStyle.Info, Variant = CBadgeVariant.Soft, Icon = "bi bi-arrow-counterclockwise", Tooltip = "Amount returned to customer")]
    Refunded
}

