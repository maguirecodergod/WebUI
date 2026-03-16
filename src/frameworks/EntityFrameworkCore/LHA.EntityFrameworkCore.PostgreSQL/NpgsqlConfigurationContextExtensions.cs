using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace LHA.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Extension methods on <see cref="LhaDbContextConfigurationContext"/> for configuring
/// the Npgsql (PostgreSQL) provider.
/// </summary>
public static class NpgsqlConfigurationContextExtensions
{
    /// <summary>
    /// Configures the <see cref="DbContextOptionsBuilder"/> in the given context to use Npgsql.
    /// Uses <see cref="LhaDbContextConfigurationContext.ExistingConnection"/> if available,
    /// otherwise falls back to <see cref="LhaDbContextConfigurationContext.ConnectionString"/>.
    /// </summary>
    public static DbContextOptionsBuilder UseNpgsql(
        this LhaDbContextConfigurationContext context,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        if (context.ExistingConnection is not null)
        {
            return context.DbContextOptions.UseNpgsql(context.ExistingConnection, builder =>
            {
                builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                npgsqlOptionsAction?.Invoke(builder);
            });
        }

        return context.DbContextOptions.UseNpgsql(context.ConnectionString, builder =>
        {
            builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            npgsqlOptionsAction?.Invoke(builder);
        });
    }
}
