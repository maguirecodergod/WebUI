using LHA.Shared.Domain.Attributes;

namespace LHA.BlazorWasm.Shared
{
    public enum CBrowserType
    {
        /// <summary>
        /// 0 - Unknown
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Unknown", Icon = "bi bi-browser-chrome")]
        Unknown,
        /// <summary>
        /// 1 - Chrome
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Chrome", Icon = "bi bi-google")]
        Chrome,
        /// <summary>
        /// 2 - Edge
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Edge", Icon = "bi bi-browser-edge")]
        Edge,
        /// <summary>
        /// 3 - Firefox
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Firefox", Icon = "bi bi-browser-firefox")]
        Firefox,
        /// <summary>
        /// 4 - Safari
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Safari", Icon = "bi bi-browser-safari")]
        Safari,
        /// <summary>
        /// 5 - Opera
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Opera", Icon = "bi bi-browser-opera")]
        Opera,
        /// <summary>
        /// 6 - Postman
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Postman", Icon = "bi bi-browser-chrome")]
        Postman,
        /// <summary>
        /// 7 - Bruno
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Bruno", Icon = "bi bi-browser-chrome")]
        Bruno,
        /// <summary>
        /// 8 - Curl
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Curl", Icon = "bi bi-terminal")]
        Curl,
        /// <summary>
        /// 9 - Grpc
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Grpc", Icon = "bi bi-lightning-charge-fill")]
        Grpc,
        /// <summary>
        /// 10 - MessageQueue
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.MessageQueue", Icon = "bi bi-mailbox2")]
        MessageQueue,
        /// <summary>
        /// 11 - BackgroundJob
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.BackgroundJob", Icon = "bi bi-cpu-fill")]
        BackgroundJob,
        /// <summary>
        /// 12 - Terminal
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Terminal", Icon = "bi bi-terminal-fill")]
        Terminal,
        /// <summary>
        /// 13 - Other
        /// </summary>
        [EnumMetadata(DisplayName = "AuditLog.Browser.Other", Icon = "bi bi-browser-chrome")]
        Other
    }
}