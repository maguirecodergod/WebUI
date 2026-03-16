using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace LHA.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Extension methods on <see cref="LhaDbContextOptions"/> for configuring
/// Npgsql (PostgreSQL) as the database provider.
/// </summary>
public static class NpgsqlDbContextOptionsExtensions
{
    /// <summary>
    /// Configures all DbContexts to use Npgsql (PostgreSQL).
    /// </summary>
    public static void UseNpgsql(
        this LhaDbContextOptions options,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        options.Configure(context =>
        {
            context.UseNpgsql(npgsqlOptionsAction);
        });
    }

    /// <summary>
    /// Configures a specific DbContext type to use Npgsql (PostgreSQL).
    /// </summary>
    public static void UseNpgsql<TDbContext>(
        this LhaDbContextOptions options,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        where TDbContext : DbContext
    {
        options.Configure<TDbContext>(context =>
        {
            context.UseNpgsql(npgsqlOptionsAction);
        });
    }
}
