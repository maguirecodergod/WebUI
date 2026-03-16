namespace LHA.Ddd.Domain;

/// <summary>
/// Wraps a domain event with metadata about when it occurred.
/// </summary>
/// <param name="Event">The domain event.</param>
/// <param name="OccurredAt">The UTC instant the event was raised.</param>
public sealed record DomainEventRecord(IDomainEvent Event, DateTimeOffset OccurredAt);
