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
            CBadgeVariant.Solid => GetSolidClass(_metadata.Style),
            CBadgeVariant.Soft => $"badge-soft-{styleName}",
            CBadgeVariant.Outline => $"badge-outline-{styleName}",
            _ => GetSolidClass(_metadata.Style)
        };

        var pillClass = (IsPill ?? _metadata.IsPill) ? "rounded-pill" : "";
        var pulseClass = (IsPulse ?? _metadata.IsPulse) ? "status-pulse" : "";

        return $"badge {variantClass} {pillClass} {pulseClass} {_metadata.CustomClass} {Class}".Trim();
    }

    private string GetSolidClass(CBadgeStyle style)
    {
        return style switch
        {
            CBadgeStyle.Primary => "bg-primary",
            CBadgeStyle.Secondary => "bg-secondary",
            CBadgeStyle.Success => "bg-success",
            CBadgeStyle.Danger => "bg-danger",
            CBadgeStyle.Warning => "bg-warning text-dark",
            CBadgeStyle.Info => "bg-info text-dark",
            CBadgeStyle.Light => "bg-light text-dark",
            CBadgeStyle.Dark => "bg-dark",
            _ => "bg-secondary"
        };
    }
}
