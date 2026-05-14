using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace LHA.EntityFrameworkCore.MongoDB;

/// <summary>
/// Extension methods on <see cref="LhaDbContextConfigurationContext"/> for configuring
/// the MongoDB provider.
/// </summary>
public static class MongoDbConfigurationContextExtensions
{
    /// <summary>
    /// Configures the <see cref="DbContextOptionsBuilder"/> in the given context to use MongoDB.
    /// Uses <see cref="LhaDbContextConfigurationContext.ConnectionString"/>.
    /// </summary>
    public static DbContextOptionsBuilder UseMongoDB(this LhaDbContextConfigurationContext context)
    {
        var mongoUrl = new MongoUrl(context.ConnectionString);
        var client = new MongoClient(mongoUrl);
        var databaseName = mongoUrl.DatabaseName;

        if (string.IsNullOrEmpty(databaseName))
        {
            throw new ArgumentException("The connection string must contain a database name.", nameof(context.ConnectionString));
        }

        return context.DbContextOptions.UseMongoDB(client, databaseName);
    }
}
