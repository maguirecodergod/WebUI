namespace LHA.BlazorWasm.Shared.Models.StatusBadge;

public enum CBadgeStyle
{
    // Core palette
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark,

    // Extended palette (NEW)
    Purple,
    Pink,
    Orange,
    Teal,
    Indigo,
    Cyan,
    Lime,
    Amber,
    Slate,

    // Neutral states
    Muted,
    Subtle,
    Contrast
}

public enum CBadgeSemantic
{
    // System
    Processing,
    Pending,
    Completed,
    Failed,
    Cancelled,
    Timeout,
    Retrying,

    // HTTP
    Http1xx,
    Http2xx,
    Http3xx,
    Http4xx,
    Http5xx,

    // Business
    Active,
    Inactive,
    Archived,
    Deleted,

    Unknown
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
    // NEW: semantic layer
    public CBadgeSemantic? Semantic { get; set; }

    // OPTIONAL override (nếu muốn force màu)
    public CBadgeStyle? Style { get; set; }

    public CBadgeVariant Variant { get; set; } = CBadgeVariant.Solid;
    public string? Icon { get; set; }
    public bool IsPulse { get; set; }
    public bool IsPill { get; set; }
    public string? Tooltip { get; set; }

    public StatusBadgeAttribute() { }

    // dùng semantic
    public StatusBadgeAttribute(CBadgeSemantic semantic)
    {
        Semantic = semantic;
    }

    // dùng style trực tiếp (legacy support)
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
