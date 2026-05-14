using LHA.Shared.Domain.Attributes;

namespace LHA.BlazorWasm.Shared
{
    public enum CBrowserType
    {
        [EnumMetadata(DisplayName = "AuditLog.Browser.Unknown", Icon = "bi bi-browser-chrome")]
        Unknown,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Chrome", Icon = "bi bi-google")]
        Chrome,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Edge", Icon = "bi bi-browser-edge")]
        Edge,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Firefox", Icon = "bi bi-browser-firefox")]
        Firefox,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Safari", Icon = "bi bi-browser-safari")]
        Safari,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Opera", Icon = "bi bi-browser-opera")]
        Opera,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Postman", Icon = "bi bi-browser-chrome")]
        Postman,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Bruno", Icon = "bi bi-browser-chrome")]
        Bruno,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Curl", Icon = "bi bi-terminal")]
        Curl,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Grpc", Icon = "bi bi-lightning-charge-fill")]
        Grpc,
        [EnumMetadata(DisplayName = "AuditLog.Browser.MessageQueue", Icon = "bi bi-mailbox2")]
        MessageQueue,
        [EnumMetadata(DisplayName = "AuditLog.Browser.BackgroundJob", Icon = "bi bi-cpu-fill")]
        BackgroundJob,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Terminal", Icon = "bi bi-terminal-fill")]
        Terminal,
        [EnumMetadata(DisplayName = "AuditLog.Browser.Other", Icon = "bi bi-browser-chrome")]
        Other
    }
}