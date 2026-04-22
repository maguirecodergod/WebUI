using LHA;

namespace LHA.BlazorWasm.Shared.Models.StatusBadge;

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
