using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Services.StatusBadge;
using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Components.Badges;

public partial class StatusBadge<TEnum> : LhaComponentBase where TEnum : struct, Enum
{
    [Inject] private IStatusBadgeService StatusBadgeService { get; set; } = default!;

    [Parameter, EditorRequired] public TEnum Status { get; set; }

    [Parameter] public string? Class { get; set; }

    [Parameter] public bool? IsPill { get; set; }
    
    [Parameter] public bool? IsPulse { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    private StatusBadgeMetadata? _metadata;

    protected override void OnParametersSet()
    {
        _metadata = StatusBadgeService.GetMetadata(Status);
    }

    private string GetBadgeClass()
    {
        if (_metadata == null) return "badge bg-secondary";

        var styleName = _metadata.Style.ToString().ToLower();
        var variantClass = _metadata.Variant switch
        {
            BadgeVariant.Solid => GetSolidClass(_metadata.Style),
            BadgeVariant.Soft => $"badge-soft-{styleName}",
            BadgeVariant.Outline => $"badge-outline-{styleName}",
            _ => GetSolidClass(_metadata.Style)
        };

        var pillClass = (IsPill ?? _metadata.IsPill) ? "rounded-pill" : "";
        var pulseClass = (IsPulse ?? _metadata.IsPulse) ? "status-pulse" : "";

        return $"badge {variantClass} {pillClass} {pulseClass} {_metadata.CustomClass} {Class}".Trim();
    }

    private string GetSolidClass(BadgeStyle style)
    {
        return style switch
        {
            BadgeStyle.Primary => "bg-primary",
            BadgeStyle.Secondary => "bg-secondary",
            BadgeStyle.Success => "bg-success",
            BadgeStyle.Danger => "bg-danger",
            BadgeStyle.Warning => "bg-warning text-dark",
            BadgeStyle.Info => "bg-info text-dark",
            BadgeStyle.Light => "bg-light text-dark",
            BadgeStyle.Dark => "bg-dark",
            _ => "bg-secondary"
        };
    }
}
