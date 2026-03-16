namespace LHA.Ddd.Domain;

/// <summary>
/// Interface for entities that support optimistic concurrency control
/// via a random concurrency stamp that changes on every persistence operation.
/// </summary>
/// <remarks>
/// This is complementary to <see cref="LHA.Auditing.IHasEntityVersion"/>:
/// <list type="bullet">
///   <item><description><c>EntityVersion</c> — monotonically increasing integer, ideal for ordering.</description></item>
///   <item><description><c>ConcurrencyStamp</c> — random token, ideal for EF Core / database-level <c>RowVersion</c> checks.</description></item>
/// </list>
/// </remarks>
public interface IHasConcurrencyStamp
{
    /// <summary>
    /// A random token that is regenerated every time the entity is persisted.
    /// Used by the persistence layer to detect concurrent modifications.
    /// </summary>
    string ConcurrencyStamp { get; set; }
}
