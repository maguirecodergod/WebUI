using LHA.BlazorWasm.Shared.Models.StatusBadge;

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
    Unpaid,
    Paid,
    Failed,
    Refunded
}
