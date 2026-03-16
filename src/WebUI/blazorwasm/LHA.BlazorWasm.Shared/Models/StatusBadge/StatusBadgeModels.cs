using System;

namespace LHA.BlazorWasm.Shared.Models.StatusBadge;

public enum CBadgeStyle
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

public enum CBadgeVariant
{
    Solid,
    Soft,
    Outline
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
public class StatusBadgeAttribute : Attribute
{
    public CBadgeStyle Style { get; set; } = CBadgeStyle.Secondary;
    public CBadgeVariant Variant { get; set; } = CBadgeVariant.Solid;
    public string? Icon { get; set; }
    public bool IsPulse { get; set; }
    public bool IsPill { get; set; }
    public string? Tooltip { get; set; }

    public StatusBadgeAttribute() { }
    
    public StatusBadgeAttribute(CBadgeStyle style)
    {
        Style = style;
    }
}

public class StatusBadgeMetadata
{
    public string? Text { get; set; }
    public CBadgeStyle Style { get; set; } = CBadgeStyle.Secondary;
    public CBadgeVariant Variant { get; set; } = CBadgeVariant.Solid;
    public bool IsPill { get; set; }
    public bool IsPulse { get; set; }
    public string? Icon { get; set; }
    public string? CustomClass { get; set; }
    public string? Tooltip { get; set; }
}
