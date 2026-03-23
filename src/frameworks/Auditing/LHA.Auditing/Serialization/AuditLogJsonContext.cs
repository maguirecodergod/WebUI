using System.Text.Json;
using System.Text.Json.Serialization;

namespace LHA.Auditing.Serialization;

/// <summary>
/// Source-generated JSON serialization context for <see cref="Pipeline.AuditLogRecord"/>
/// and related types. Provides AOT-compatible, zero-reflection serialization.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(Pipeline.AuditLogRecord))]
[JsonSerializable(typeof(Pipeline.AuditLogRecord[]))]
[JsonSerializable(typeof(List<Pipeline.AuditLogRecord>))]
[JsonSerializable(typeof(AuditExceptionInfo))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class AuditLogJsonContext : JsonSerializerContext;
