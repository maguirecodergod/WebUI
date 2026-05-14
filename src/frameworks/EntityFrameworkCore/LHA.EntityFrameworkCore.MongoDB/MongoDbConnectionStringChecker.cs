using MongoDB.Driver;

namespace LHA.EntityFrameworkCore.MongoDB;

/// <summary>
/// Validates a MongoDB connection string by attempting to connect.
/// </summary>
public static class MongoDbConnectionStringChecker
{
    /// <summary>
    /// Checks whether the specified connection string can connect to the MongoDB server
    /// and whether the target database exists.
    /// </summary>
    /// <returns>A result indicating connectivity and database existence.</returns>
    public static async Task<MongoDbConnectionCheckResult> CheckAsync(string connectionString)
    {
        var result = new MongoDbConnectionCheckResult();

        try
        {
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            
            var pingCommand = new global::MongoDB.Bson.BsonDocument("ping", 1);
            
            // Checking connectivity by pinging the admin database
            await client.GetDatabase("admin").RunCommandAsync<global::MongoDB.Bson.BsonDocument>(pingCommand);
            result.Connected = true;

            var databaseName = mongoUrl.DatabaseName;
            if (!string.IsNullOrEmpty(databaseName))
            {
                // Check if database exists
                using var cursor = await client.ListDatabaseNamesAsync();
                var databases = await cursor.ToListAsync();
                if (databases.Contains(databaseName))
                {
                    result.DatabaseExists = true;
                }
            }
        }
        catch
        {
            // Connection or database check failed — result defaults apply.
        }

        return result;
    }
}

/// <summary>
/// Result of a MongoDB connection string check.
/// </summary>
public sealed class MongoDbConnectionCheckResult
{
    /// <summary>Whether the server was reachable.</summary>
    public bool Connected { get; set; }

    /// <summary>Whether the target database exists.</summary>
    public bool DatabaseExists { get; set; }
}
