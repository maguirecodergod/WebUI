using LHA.Shared.Domain.Attributes;

namespace LHA.BlazorWasm.Shared
{
    public enum COperatingSystem
    {
        [EnumMetadata(DisplayName = "AuditLog.OS.Unknown", Icon = "bi bi-laptop")]
        Unknown,
        [EnumMetadata(DisplayName = "AuditLog.OS.Windows", Icon = "bi bi-windows")]
        Windows,
        [EnumMetadata(DisplayName = "AuditLog.OS.MacOS", Icon = "bi bi-apple")]
        MacOS,
        [EnumMetadata(DisplayName = "AuditLog.OS.Linux", Icon = "bi bi-ubuntu")]
        Linux,
        [EnumMetadata(DisplayName = "AuditLog.OS.Android", Icon = "bi bi-android2")]
        Android,
        [EnumMetadata(DisplayName = "AuditLog.OS.iOS", Icon = "bi bi-apple")]
        iOS
    }
}