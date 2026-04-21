using Microsoft.AspNetCore.Components;
using LHA.BlazorWasm.Shared.Models.StatusBadge;
using System.Reflection;
using System.Collections.Concurrent;
using LHA.BlazorWasm.Shared.Helpers;

namespace LHA.BlazorWasm.Components.Badges;

public partial class StatusBadge<TEnum> : LHAComponentBase where TEnum : struct, Enum
{
    private static readonly ConcurrentDictionary<(Type, object), StatusBadgeMetadata> _cache = new();

    [Parameter, EditorRequired] public TEnum Status { get; set; }

    [Parameter] public string? Class { get; set; }

    [Parameter] public string? Style { get; set; }

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

        var memInfo = typeof(TEnum).GetMember(value.ToString());
        var attribute = memInfo.Length > 0 ? memInfo[0].GetCustomAttribute<StatusBadgeAttribute>() : null;

        if (attribute != null)
        {
            // Ưu tiên Style override
            if (attribute.Style.HasValue)
            {
                metadata.Style = attribute.Style.Value;
            }
            // Nếu không có → dùng Semantic
            else if (attribute.Semantic.HasValue)
            {
                metadata.Style = attribute.Semantic.Value.ToStyle();
            }
            else
            {
                metadata.Style = CBadgeStyle.Secondary;
            }

            metadata.Variant = attribute.Variant;
            metadata.Icon = attribute.Icon;
            metadata.IsPulse = attribute.IsPulse;
            metadata.IsPill = attribute.IsPill;
            metadata.Tooltip = attribute.Tooltip;
        }
        else
        {
            // fallback thông minh hơn
            var intValue = Convert.ToInt32(value);

            metadata.Style = intValue switch
            {
                >= 100 and < 200 => CBadgeSemantic.Http1xx.ToStyle(),
                >= 200 and < 300 => CBadgeSemantic.Http2xx.ToStyle(),
                >= 300 and < 400 => CBadgeSemantic.Http3xx.ToStyle(),
                >= 400 and < 500 => CBadgeSemantic.Http4xx.ToStyle(),
                >= 500 => CBadgeSemantic.Http5xx.ToStyle(),
                _ => CBadgeStyle.Muted
            };

            metadata.Variant = CBadgeVariant.Soft;
        }

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

        var pillClass = (IsPill ?? _metadata.IsPill) ? "rounded-pill" : string.Empty;
        var pulseClass = (IsPulse ?? _metadata.IsPulse) ? "status-pulse" : string.Empty;

        return $"badge {variantClass} {pillClass} {pulseClass} {_metadata.CustomClass} {Class}".Trim();
    }

    private string GetSolidClass(CBadgeStyle style)
    {
        return style switch
        {
            // Core palette
            CBadgeStyle.Primary => "bg-primary",
            CBadgeStyle.Secondary => "bg-secondary",
            CBadgeStyle.Success => "bg-success",
            CBadgeStyle.Danger => "bg-danger",
            CBadgeStyle.Warning => "bg-warning text-dark",
            CBadgeStyle.Info => "bg-info text-dark",
            CBadgeStyle.Light => "bg-light text-dark",
            CBadgeStyle.Dark => "bg-dark",

            // Extended palette
            CBadgeStyle.Purple => "bg-purple text-white",
            CBadgeStyle.Pink => "bg-pink text-white",
            CBadgeStyle.Orange => "bg-orange text-white",
            CBadgeStyle.Teal => "bg-teal text-white",
            CBadgeStyle.Indigo => "bg-indigo text-white",
            CBadgeStyle.Cyan => "bg-cyan text-dark",
            CBadgeStyle.Lime => "bg-lime text-dark",
            CBadgeStyle.Amber => "bg-amber text-dark",
            CBadgeStyle.Slate => "bg-slate text-white",

            // Neutral
            CBadgeStyle.Muted => "bg-secondary text-white",
            CBadgeStyle.Subtle => "bg-light text-muted",
            CBadgeStyle.Contrast => "bg-dark text-white",

            _ => "bg-secondary"
        };
    }
}