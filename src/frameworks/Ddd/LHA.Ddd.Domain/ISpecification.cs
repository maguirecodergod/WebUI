using System.Linq.Expressions;

namespace LHA.Ddd.Domain;

/// <summary>
/// Represents a business rule that can be evaluated against a candidate object.
/// </summary>
/// <typeparam name="T">The type of object the specification applies to.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Determines whether the specified candidate satisfies this specification.
    /// </summary>
    bool IsSatisfiedBy(T candidate);

    /// <summary>
    /// Returns a LINQ expression representing this specification, suitable for use with
    /// <c>IQueryable&lt;T&gt;</c> providers such as Entity Framework Core.
    /// </summary>
    Expression<Func<T, bool>> ToExpression();
}
