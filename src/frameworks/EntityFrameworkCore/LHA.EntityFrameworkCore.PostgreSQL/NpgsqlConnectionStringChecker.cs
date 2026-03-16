using Npgsql;

namespace LHA.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Validates a PostgreSQL connection string by attempting to connect.
/// </summary>
public static class NpgsqlConnectionStringChecker
{
    /// <summary>
    /// Checks whether the specified connection string can connect to the PostgreSQL server
    /// and whether the target database exists.
    /// </summary>
    /// <returns>A result indicating connectivity and database existence.</returns>
    public static async Task<NpgsqlConnectionCheckResult> CheckAsync(string connectionString)
    {
        var result = new NpgsqlConnectionCheckResult();

        try
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Timeout = 3
            };

            var targetDatabase = builder.Database;
            builder.Database = "postgres";

            await using var connection = new NpgsqlConnection(builder.ConnectionString);
            await connection.OpenAsync();
            result.Connected = true;

            if (!string.IsNullOrEmpty(targetDatabase))
            {
                await connection.ChangeDatabaseAsync(targetDatabase);
                result.DatabaseExists = true;
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
/// Result of a PostgreSQL connection string check.
/// </summary>
public sealed class NpgsqlConnectionCheckResult
{
    /// <summary>Whether the server was reachable.</summary>
    public bool Connected { get; set; }

    /// <summary>Whether the target database exists.</summary>
    public bool DatabaseExists { get; set; }
}
