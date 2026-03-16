namespace LHA.EventBus;

/// <summary>
/// Overrides the canonical event name used during serialization and routing.
/// <para>
/// Without this attribute, the full type name is used. Apply this attribute
/// to decouple the wire name from the CLR type, enabling safe refactoring
/// and cross-language interoperability.
/// </para>
/// </summary>
/// <example>
/// <code>
/// [EventName("OrderService.OrderPlaced")]
/// public sealed record OrderPlaced : IntegrationEvent { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventNameAttribute(string name) : Attribute
{
    /// <summary>The canonical event name for transport routing.</summary>
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
}
