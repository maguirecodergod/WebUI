using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Shared.Models.StatusBadge;
using System.Reflection;
using System.Collections.Concurrent;

namespace LHA.BlazorWasm.Components.Badges;

public partial class StatusBadge<TEnum> : LhaComponentBase where TEnum : struct, Enum
{
    private static readonly ConcurrentDictionary<(Type, object), StatusBadgeMetadata> _cache = new();

    [Parameter, EditorRequired] public TEnum Status { get; set; }

    [Parameter] public string? Class { get; set; }

    [Parameter] public bool? IsPill { get; set; }
    
    [Parameter] public bool? IsPulse { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    private StatusBadgeMetadata? _metadata;

    protected override void OnParametersSet()
    {
        _metadata = GetMetadata(Status);
    }

    private StatusBadgeMetadata GetMetadata(TEnum value)
    {
        return _cache.GetOrAdd((typeof(TEnum), value), _ => ResolveMetadata(value));
    }

    private StatusBadgeMetadata ResolveMetadata(TEnum value)
    {
        var metadata = new StatusBadgeMetadata();
        
        // 1. Check for StatusBadgeAttribute
        var memInfo = typeof(TEnum).GetMember(value.ToString());
        var attribute = memInfo.Length > 0 ? memInfo[0].GetCustomAttribute<StatusBadgeAttribute>() : null;

        if (attribute != null)
        {
            metadata.Style = attribute.Style;
            metadata.Variant = attribute.Variant;
            metadata.Icon = attribute.Icon;
            metadata.IsPulse = attribute.IsPulse;
            metadata.IsPill = attribute.IsPill;
            metadata.Tooltip = attribute.Tooltip;
        }
        else
        {
            // 2. Fallback to convention
            var name = value.ToString().ToLower();
            if (name.Contains("success") || name.Contains("paid") || name.Contains("completed") || name.Contains("active"))
            {
                metadata.Style = CBadgeStyle.Success;
                metadata.Variant = CBadgeVariant.Soft;
            }
            else if (name.Contains("error") || name.Contains("fail") || name.Contains("cancel") || name.Contains("deleted"))
            {
                metadata.Style = CBadgeStyle.Danger;
                metadata.Variant = CBadgeVariant.Soft;
            }
            else if (name.Contains("pending") || name.Contains("warning") || name.Contains("processing"))
            {
                metadata.Style = CBadgeStyle.Warning;
                metadata.Variant = CBadgeVariant.Soft;
            }
            else
            {
                metadata.Style = CBadgeStyle.Secondary;
            }
        }

        // 3. Localization
        metadata.Text = L(value.ToString());

        return metadata;
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

