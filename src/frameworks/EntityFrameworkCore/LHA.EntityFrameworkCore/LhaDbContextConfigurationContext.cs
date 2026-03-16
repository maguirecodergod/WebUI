using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Context passed to DbContext configuration actions, containing the connection information
/// and EF Core options builder.
/// </summary>
public sealed class LhaDbContextConfigurationContext
{
    /// <summary>
    /// The connection string to use for the DbContext.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    /// An existing <see cref="DbConnection"/> to use instead of <see cref="ConnectionString"/>.
    /// When set, the provider should use this connection rather than creating a new one.
    /// </summary>
    public DbConnection? ExistingConnection { get; init; }

    /// <summary>
    /// The EF Core <see cref="DbContextOptionsBuilder"/> to configure.
    /// </summary>
    public required DbContextOptionsBuilder DbContextOptions { get; init; }
}
