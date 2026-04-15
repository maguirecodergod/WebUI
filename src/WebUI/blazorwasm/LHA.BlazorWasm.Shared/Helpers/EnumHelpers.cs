using LHA.BlazorWasm.Shared.Models.StatusBadge;

namespace LHA.BlazorWasm.Shared.Helpers
{
    public static class EnumHelpers
    {
        public static CBadgeStyle ToStyle(this CBadgeSemantic semantic) => semantic switch
        {
            // System
            CBadgeSemantic.Processing => CBadgeStyle.Info,
            CBadgeSemantic.Pending => CBadgeStyle.Warning,
            CBadgeSemantic.Completed => CBadgeStyle.Success,
            CBadgeSemantic.Failed => CBadgeStyle.Danger,
            CBadgeSemantic.Cancelled => CBadgeStyle.Secondary,
            CBadgeSemantic.Timeout => CBadgeStyle.Dark,
            CBadgeSemantic.Retrying => CBadgeStyle.Amber,

            // HTTP
            CBadgeSemantic.Http1xx => CBadgeStyle.Cyan,
            CBadgeSemantic.Http2xx => CBadgeStyle.Success,
            CBadgeSemantic.Http3xx => CBadgeStyle.Indigo,
            CBadgeSemantic.Http4xx => CBadgeStyle.Orange,
            CBadgeSemantic.Http5xx => CBadgeStyle.Danger,

            // HTTP Method
            CBadgeSemantic.HttpGet => CBadgeStyle.Success,
            CBadgeSemantic.HttpPost => CBadgeStyle.Primary,
            CBadgeSemantic.HttpPut => CBadgeStyle.Info,
            CBadgeSemantic.HttpPatch => CBadgeStyle.Teal,
            CBadgeSemantic.HttpDelete => CBadgeStyle.Danger,
            CBadgeSemantic.HttpHead => CBadgeStyle.Secondary,
            CBadgeSemantic.HttpOptions => CBadgeStyle.Indigo,
            CBadgeSemantic.HttpTrace => CBadgeStyle.Pink,
            CBadgeSemantic.HttpConnect => CBadgeStyle.Purple,

            // Business
            CBadgeSemantic.Active => CBadgeStyle.Success,
            CBadgeSemantic.Inactive => CBadgeStyle.Slate,
            CBadgeSemantic.Archived => CBadgeStyle.Dark,
            CBadgeSemantic.Deleted => CBadgeStyle.Danger,

            _ => CBadgeStyle.Muted
        };
    }
}