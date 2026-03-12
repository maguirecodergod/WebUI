using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Shared.Models;

public enum OrderStatus
{
    [StatusBadge(BadgeStyle.Warning, Variant = BadgeVariant.Soft, Icon = "bi bi-clock")]
    Pending,

    [StatusBadge(BadgeStyle.Primary, Variant = BadgeVariant.Soft, Icon = "bi bi-gear-fill", IsPulse = true)]
    Processing,

    [StatusBadge(BadgeStyle.Success, Variant = BadgeVariant.Soft, Icon = "bi bi-check-circle-fill")]
    Completed,

    [StatusBadge(BadgeStyle.Danger, Variant = BadgeVariant.Soft, Icon = "bi bi-x-circle-fill")]
    Cancelled
}

/// <summary>
/// Demonstrating Convention-Based resolution. 
/// Even without attributes, "Paid" will be Success and "Failed" will be Danger.
/// </summary>
public enum PaymentStatus
{
    Unpaid,
    Paid,
    Failed,
    Refunded
}
