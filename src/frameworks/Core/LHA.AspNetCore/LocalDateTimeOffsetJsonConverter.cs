using System.Text.Json;
using System.Text.Json.Serialization;

namespace LHA.AspNetCore;

/// <summary>
/// JSON converter that serialises <see cref="DateTimeOffset"/> values using the
/// server's local time zone so that API consumers receive local timestamps.
/// <para>
/// Read (deserialise) side: accepts any valid ISO-8601 / RFC-3339 string and
/// parses it as-is (preserves the original offset).
/// </para>
/// <para>
/// Write (serialise) side: converts every <see cref="DateTimeOffset"/> to local
/// time before writing, producing strings like <c>"2026-03-05T19:30:00+07:00"</c>.
/// </para>
/// </summary>
public sealed class LocalDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    /// <inheritdoc />
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetDateTimeOffset();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        // Convert to server-local time so the offset reflects the local zone
        var local = value.ToLocalTime();
        writer.WriteStringValue(local);
    }
}

/// <summary>
/// Handles <see cref="Nullable{DateTimeOffset}"/> — delegates to
/// <see cref="LocalDateTimeOffsetJsonConverter"/> for non-null values.
/// </summary>
public sealed class NullableLocalDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
{
    private static readonly LocalDateTimeOffsetJsonConverter Inner = new();

    /// <inheritdoc />
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        return Inner.Read(ref reader, typeof(DateTimeOffset), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        Inner.Write(writer, value.Value, options);
    }
}
