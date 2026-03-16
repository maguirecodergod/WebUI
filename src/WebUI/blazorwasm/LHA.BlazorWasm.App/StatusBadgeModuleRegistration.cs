using LHA.BlazorWasm.Services.StatusBadge;
using LHA.BlazorWasm.Shared.Models;
using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.App;

public static class StatusBadgeModuleRegistration
{
    public static void RegisterOrderModuleMappings(this IServiceProvider serviceProvider)
    {
        serviceProvider.RegisterBadgeMappings<COrderStatus>(builder =>
        {
            builder.Map(COrderStatus.Pending, m => { m.Style = CBadgeStyle.Warning; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-clock"; });
            builder.Map(COrderStatus.Processing, m => { m.Style = CBadgeStyle.Primary; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-gear-fill"; m.IsPulse = true; });
            builder.Map(COrderStatus.Completed, m => { m.Style = CBadgeStyle.Success; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-check-circle-fill"; });
            builder.Map(COrderStatus.Cancelled, m => { m.Style = CBadgeStyle.Danger; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-x-circle-fill"; });
        });
    }

    public static void RegisterPaymentModuleMappings(this IServiceProvider serviceProvider)
    {
        serviceProvider.RegisterBadgeMappings<CPaymentStatus>(builder =>
        {
            builder.Map(CPaymentStatus.Unpaid, m => { m.Style = CBadgeStyle.Secondary; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-cash"; m.Tooltip = "Awaiting payment"; });
            builder.Map(CPaymentStatus.Paid, m => { m.Style = CBadgeStyle.Success; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-currency-dollar"; m.Tooltip = "Payment confirmed"; });
            builder.Map(CPaymentStatus.Failed, m => { m.Style = CBadgeStyle.Danger; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-exclamation-triangle-fill"; m.Tooltip = "Transaction failed"; });
            builder.Map(CPaymentStatus.Refunded, m => { m.Style = CBadgeStyle.Info; m.Variant = CBadgeVariant.Soft; m.Icon = "bi bi-arrow-counterclockwise"; m.Tooltip = "Amount returned to customer"; });
        });
    }
}
