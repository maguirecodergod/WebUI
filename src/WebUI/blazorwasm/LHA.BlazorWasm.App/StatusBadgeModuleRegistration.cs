using LHA.BlazorWasm.Services.StatusBadge;
using LHA.BlazorWasm.Shared.Models;
using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.App;

public static class StatusBadgeModuleRegistration
{
    public static void RegisterOrderModuleMappings(this IServiceProvider serviceProvider)
    {
        serviceProvider.RegisterBadgeMappings<OrderStatus>(builder =>
        {
            builder.Map(OrderStatus.Pending, m => { m.Style = BadgeStyle.Warning; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-clock"; });
            builder.Map(OrderStatus.Processing, m => { m.Style = BadgeStyle.Primary; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-gear-fill"; m.IsPulse = true; });
            builder.Map(OrderStatus.Completed, m => { m.Style = BadgeStyle.Success; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-check-circle-fill"; });
            builder.Map(OrderStatus.Cancelled, m => { m.Style = BadgeStyle.Danger; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-x-circle-fill"; });
        });
    }

    public static void RegisterPaymentModuleMappings(this IServiceProvider serviceProvider)
    {
        serviceProvider.RegisterBadgeMappings<PaymentStatus>(builder =>
        {
            builder.Map(PaymentStatus.Unpaid, m => { m.Style = BadgeStyle.Secondary; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-cash"; m.Tooltip = "Awaiting payment"; });
            builder.Map(PaymentStatus.Paid, m => { m.Style = BadgeStyle.Success; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-currency-dollar"; m.Tooltip = "Payment confirmed"; });
            builder.Map(PaymentStatus.Failed, m => { m.Style = BadgeStyle.Danger; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-exclamation-triangle-fill"; m.Tooltip = "Transaction failed"; });
            builder.Map(PaymentStatus.Refunded, m => { m.Style = BadgeStyle.Info; m.Variant = BadgeVariant.Soft; m.Icon = "bi bi-arrow-counterclockwise"; m.Tooltip = "Amount returned to customer"; });
        });
    }
}
