using System;

namespace LHA;

/// <summary>
/// Attribute used to decorate enum members with UI metadata for badge rendering.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class StatusBadgeAttribute : Attribute
{
    public CBadgeSemantic? Semantic { get; }
    public CBadgeStyle? Style { get; set; }
    public CBadgeVariant Variant { get; set; } = CBadgeVariant.Soft;
    public string? Icon { get; set; }
    public string? Label { get; set; }
    public bool IsPulse { get; set; }
    public bool IsPill { get; set; }
    public string? Tooltip { get; set; }

    public StatusBadgeAttribute(CBadgeSemantic semantic)
    {
        Semantic = semantic;
    }

    public StatusBadgeAttribute(CBadgeStyle style)
    {
        Style = style;
    }
}
