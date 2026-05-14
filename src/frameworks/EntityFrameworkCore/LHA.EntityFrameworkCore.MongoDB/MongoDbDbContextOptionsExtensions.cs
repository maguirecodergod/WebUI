using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore.MongoDB;

/// <summary>
/// Extension methods on <see cref="LhaDbContextOptions"/> for configuring
/// MongoDB as the database provider.
/// </summary>
public static class MongoDbDbContextOptionsExtensions
{
    /// <summary>
    /// Configures all DbContexts to use MongoDB.
    /// </summary>
    public static void UseMongoDB(this LhaDbContextOptions options)
    {
        options.Configure(context =>
        {
            context.UseMongoDB();
        });
    }

    /// <summary>
    /// Configures a specific DbContext type to use MongoDB.
    /// </summary>
    public static void UseMongoDB<TDbContext>(this LhaDbContextOptions options)
        where TDbContext : DbContext
    {
        options.Configure<TDbContext>(context =>
        {
            context.UseMongoDB();
        });
    }
}
