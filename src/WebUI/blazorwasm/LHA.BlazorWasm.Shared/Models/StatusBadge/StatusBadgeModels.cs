using System;

namespace LHA.BlazorWasm.Shared.Models.StatusBadge;

public enum BadgeStyle
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark
}

public enum BadgeVariant
{
    Solid,
    Soft,
    Outline
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
public class StatusBadgeAttribute : Attribute
{
    public BadgeStyle Style { get; set; } = BadgeStyle.Secondary;
    public BadgeVariant Variant { get; set; } = BadgeVariant.Solid;
    public string? Icon { get; set; }
    public bool IsPulse { get; set; }
    public bool IsPill { get; set; }
    public string? Tooltip { get; set; }

    public StatusBadgeAttribute() { }
    
    public StatusBadgeAttribute(BadgeStyle style)
    {
        Style = style;
    }
}

public class StatusBadgeMetadata
{
    public string? Text { get; set; }
    public BadgeStyle Style { get; set; } = BadgeStyle.Secondary;
    public BadgeVariant Variant { get; set; } = BadgeVariant.Solid;
    public bool IsPill { get; set; }
    public bool IsPulse { get; set; }
    public string? Icon { get; set; }
    public string? CustomClass { get; set; }
    public string? Tooltip { get; set; }
}
