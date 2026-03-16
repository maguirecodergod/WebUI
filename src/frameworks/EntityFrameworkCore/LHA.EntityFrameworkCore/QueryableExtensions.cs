using System.Linq.Expressions;
using System.Reflection;
using LHA.Core;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Generic <see cref="IQueryable{T}"/> extension methods for dynamic search, filter,
/// and sorting via expression trees.
/// <para>
/// These extensions are designed to work with any EF Core entity. They build
/// <see cref="Expression"/> trees at runtime so the resulting predicates are
/// translated to SQL by the EF Core provider.
/// </para>
/// </summary>
public static class QueryableExtensions
{
    // ──────────────────────────────────────────────────────────────────
    //  SearchDynamic
    // ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Applies a dynamic keyword search across the specified <paramref name="searchColumns"/>.
    /// <para>
    /// The <paramref name="keyword"/> is split by whitespace. Each token is matched
    /// against every column using <paramref name="searchOperator"/>. Tokens are
    /// combined via <paramref name="combineMode"/> (default: OR — any token matches).
    /// </para>
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="source">Source queryable.</param>
    /// <param name="keyword">
    /// Space-separated search text. <c>null</c> / empty returns the source unchanged.
    /// </param>
    /// <param name="searchColumns">
    /// Property names to search (case-insensitive match against <typeparamref name="T"/>).
    /// Non-string properties are converted via <c>ToString()</c>.
    /// </param>
    /// <param name="searchOperator">
    /// Comparison mode: <see cref="CSearchOperatorType.Contains"/> (default),
    /// <see cref="CSearchOperatorType.Equals"/>, or <see cref="CSearchOperatorType.StartsWith"/>.
    /// </param>
    /// <param name="ignoreCase">
    /// When <c>true</c> (default), both the property value and keyword are lower-cased before comparison.
    /// </param>
    /// <param name="combineMode">
    /// How multiple keywords are combined:
    /// <see cref="CSearchCombineModeType.Or"/> (default) — any keyword hit,
    /// <see cref="CSearchCombineModeType.And"/> — all keywords must hit.
    /// </param>
    /// <returns>Filtered queryable.</returns>
    public static IQueryable<T> SearchDynamic<T>(
        this IQueryable<T> source,
        string? keyword,
        IEnumerable<string> searchColumns,
        CSearchOperatorType searchOperator = CSearchOperatorType.Contains,
        bool ignoreCase = true,
        CSearchCombineModeType combineMode = CSearchCombineModeType.Or)
    {
        if (string.IsNullOrWhiteSpace(keyword) || searchColumns is null || !searchColumns.Any())
            return source;

        var keywords = keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (keywords.Length == 0)
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? finalCombined = null;

        foreach (var word in keywords)
        {
            var wordExpr = BuildPerKeywordExpression<T>(
                parameter, word, searchColumns, searchOperator, ignoreCase);

            if (wordExpr is null)
                continue;

            finalCombined = finalCombined is null
                ? wordExpr
                : combineMode == CSearchCombineModeType.And
                    ? Expression.AndAlso(finalCombined, wordExpr)
                    : Expression.OrElse(finalCombined, wordExpr);
        }

        if (finalCombined is null)
            return source;

        var lambda = Expression.Lambda<Func<T, bool>>(finalCombined, parameter);
        return source.Where(lambda);
    }

    // ──────────────────────────────────────────────────────────────────
    //  OrderByDynamic
    // ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Applies dynamic ordering by a single property name.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="source">Source queryable.</param>
    /// <param name="propertyName">
    /// Property name (case-insensitive).
    /// </param>
    /// <param name="ascending">Sort direction.</param>
    /// <returns>Ordered queryable.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="propertyName"/> does not match a public instance property on <typeparamref name="T"/>.
    /// </exception>
    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> source,
        string propertyName,
        bool ascending = true)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return source;

        var parameter = Expression.Parameter(typeof(T), "x");

        var property = typeof(T).GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
            ?? throw new ArgumentException(
                $"Property '{propertyName}' not found on type '{typeof(T).Name}'.",
                nameof(propertyName));

        var propertyAccess = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);

        var methodName = ascending ? "OrderBy" : "OrderByDescending";

        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType);

        var result = method.Invoke(null, [source, lambda]);

        return result is IQueryable<T> orderedQuery
            ? orderedQuery
            : throw new InvalidOperationException(
                $"Failed to apply '{methodName}' on type '{typeof(T).Name}'.");
    }

    /// <summary>
    /// Parses a sorting string in the format <c>"propertyName [asc|desc]"</c> and
    /// applies dynamic ordering. Falls back to <paramref name="defaultProperty"/> when
    /// <paramref name="sorting"/> is <c>null</c> or empty.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="source">Source queryable.</param>
    /// <param name="sorting">
    /// Sorting expression, e.g. <c>"Name desc"</c>, <c>"CreationTime"</c>.
    /// Defaults to ascending when direction is omitted.
    /// </param>
    /// <param name="defaultProperty">
    /// Fallback property name when <paramref name="sorting"/> is empty.
    /// </param>
    /// <param name="defaultAscending">
    /// Fallback direction when <paramref name="sorting"/> is empty.
    /// </param>
    /// <returns>Ordered queryable.</returns>
    public static IQueryable<T> SortByDynamic<T>(
        this IQueryable<T> source,
        string? sorting,
        string defaultProperty = "Id",
        bool defaultAscending = true)
    {
        if (string.IsNullOrWhiteSpace(sorting))
            return source.OrderByDynamic(defaultProperty, defaultAscending);

        var parts = sorting.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var propertyName = parts[0];
        var ascending = parts.Length < 2
            || !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

        return source.OrderByDynamic(propertyName, ascending);
    }

    // ──────────────────────────────────────────────────────────────────
    //  WhereIf (conditional filter)
    // ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Appends a <c>Where</c> clause only when <paramref name="condition"/> is <c>true</c>.
    /// </summary>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    // ──────────────────────────────────────────────────────────────────
    //  Private helpers
    // ──────────────────────────────────────────────────────────────────

    private static Expression? BuildPerKeywordExpression<T>(
        ParameterExpression parameter,
        string keyword,
        IEnumerable<string> searchColumns,
        CSearchOperatorType searchOperator,
        bool ignoreCase)
    {
        Expression? combined = null;
        var processedKeyword = ignoreCase ? keyword.ToLowerInvariant() : keyword;

        foreach (var columnName in searchColumns)
        {
            var property = typeof(T).GetProperty(columnName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property is null)
                continue;

            var propertyExpr = Expression.Property(parameter, property);

            // Convert non-string properties to string via ToString()
            Expression stringExpr = property.PropertyType == typeof(string)
                ? propertyExpr
                : Expression.Call(propertyExpr, nameof(object.ToString), Type.EmptyTypes);

            if (ignoreCase)
            {
                stringExpr = Expression.Call(stringExpr,
                    typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!);
            }

            var keywordExpr = Expression.Constant(processedKeyword, typeof(string));

            var condition = searchOperator switch
            {
                CSearchOperatorType.Equals =>
                    Expression.Call(stringExpr,
                        typeof(string).GetMethod(nameof(string.Equals), [typeof(string)])!,
                        keywordExpr),

                CSearchOperatorType.StartsWith =>
                    Expression.Call(stringExpr,
                        typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!,
                        keywordExpr),

                _ => // Contains (default)
                    Expression.Call(stringExpr,
                        typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                        keywordExpr),
            };

            combined = combined is null ? condition : Expression.OrElse(combined, condition);
        }

        return combined;
    }
}
