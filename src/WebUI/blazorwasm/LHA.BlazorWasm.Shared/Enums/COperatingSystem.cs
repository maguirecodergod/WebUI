using LHA.Shared.Domain.Attributes;

namespace LHA.BlazorWasm.Shared
{
    public enum COperatingSystem
    {
        /// <summary>
        /// 0 - Unknown
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.OS.Unknown", Icon = "bi bi-laptop")]
        Unknown,
        /// <summary>
        /// 1 - Windows
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.OS.Windows", Icon = "bi bi-windows")]
        Windows,
        /// <summary>
        /// 2 - MacOS
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.OS.MacOS", Icon = "bi bi-apple")]
        MacOS,
        /// <summary>
        /// 3 - Linux
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.OS.Linux", Icon = "bi bi-ubuntu")]
        Linux,
        /// <summary>
        /// 4 - Android
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.OS.Android", Icon = "bi bi-android2")]
        Android,
        /// <summary>
        /// 5 - iOS
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.OS.iOS", Icon = "bi bi-apple")]
        iOS
    }
}