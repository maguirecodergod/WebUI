using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LHA.EntityFrameworkCore;

/// <summary>
/// Extension methods on <see cref="ModelBuilder"/> to configure the
/// <see cref="OutboxMessage"/> and <see cref="InboxMessage"/> tables.
/// <para>
/// Call <see cref="TryConfigureEventBus"/> in <c>OnModelCreating</c> to auto-detect
/// <see cref="IHasEventOutbox"/> / <see cref="IHasEventInbox"/> on the DbContext
/// and configure the corresponding tables only when opted in.
/// </para>
/// </summary>
public static class ModelBuilderEventBusExtensions
{
    /// <summary>Default table name for outbox messages.</summary>
    public const string DefaultOutboxTableName = "__OutboxMessages";

    /// <summary>Default table name for inbox messages.</summary>
    public const string DefaultInboxTableName = "__InboxMessages";

    /// <summary>
    /// Auto-detects <see cref="IHasEventOutbox"/> and <see cref="IHasEventInbox"/>
    /// on <typeparamref name="TDbContext"/> and configures the matching tables.
    /// <para>
    /// This is the recommended entry point — call it from your DbContext's
    /// <c>OnModelCreating</c> so the tables are only added when the DbContext
    /// implements the corresponding marker interface.
    /// </para>
    /// </summary>
    public static ModelBuilder TryConfigureEventBus<TDbContext>(
        this ModelBuilder modelBuilder,
        string? schema = null)
        where TDbContext : DbContext
    {
        if (typeof(IHasEventOutbox).IsAssignableFrom(typeof(TDbContext)))
            modelBuilder.ConfigureEventOutbox(schema: schema);

        if (typeof(IHasEventInbox).IsAssignableFrom(typeof(TDbContext)))
            modelBuilder.ConfigureEventInbox(schema: schema);

        return modelBuilder;
    }

    /// <summary>
    /// Configures the <see cref="OutboxMessage"/> table with proper column types, indices,
    /// and optional customizations.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="tableName">Override the default table name.</param>
    /// <param name="schema">Optional database schema.</param>
    /// <param name="configureEntity">Optional additional entity configuration.</param>
    public static ModelBuilder ConfigureEventOutbox(
        this ModelBuilder modelBuilder,
        string tableName = DefaultOutboxTableName,
        string? schema = null,
        Action<EntityTypeBuilder<OutboxMessage>>? configureEntity = null)
    {
        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable(tableName, schema);

            b.HasKey(m => m.Id);
            b.Property(m => m.Id).ValueGeneratedNever();

            b.Property(m => m.EventName).IsRequired().HasMaxLength(256);
            b.Property(m => m.EventVersion).IsRequired().HasDefaultValue(1);
            b.Property(m => m.Payload).IsRequired();
            b.Property(m => m.MetadataJson).HasColumnType("jsonb");
            b.Property(m => m.CreatedAtUtc).IsRequired();
            b.Property(m => m.ProcessedAtUtc);
            b.Property(m => m.RetryCount).IsRequired().HasDefaultValue(0);
            b.Property(m => m.PartitionKey).HasMaxLength(128);

            // Filtered index for the background processor to pick up pending messages.
            b.HasIndex(m => m.CreatedAtUtc)
                .HasFilter($"\"{nameof(OutboxMessage.ProcessedAtUtc)}\" IS NULL")
                .HasDatabaseName("IX_OutboxMessages_Pending");

            configureEntity?.Invoke(b);
        });

        return modelBuilder;
    }

    /// <summary>
    /// Configures the <see cref="InboxMessage"/> table with proper column types, indices,
    /// and optional customizations.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <param name="tableName">Override the default table name.</param>
    /// <param name="schema">Optional database schema.</param>
    /// <param name="configureEntity">Optional additional entity configuration.</param>
    public static ModelBuilder ConfigureEventInbox(
        this ModelBuilder modelBuilder,
        string tableName = DefaultInboxTableName,
        string? schema = null,
        Action<EntityTypeBuilder<InboxMessage>>? configureEntity = null)
    {
        modelBuilder.Entity<InboxMessage>(b =>
        {
            b.ToTable(tableName, schema);

            b.HasKey(m => m.Id);
            b.Property(m => m.Id).ValueGeneratedNever();

            b.Property(m => m.EventId).IsRequired();
            b.Property(m => m.EventName).IsRequired().HasMaxLength(256);
            b.Property(m => m.ConsumerGroup).IsRequired().HasMaxLength(256);
            b.Property(m => m.ReceivedAtUtc).IsRequired();
            b.Property(m => m.ProcessedAtUtc);

            // Unique composite index for deduplication: (EventId, ConsumerGroup).
            b.HasIndex(m => new { m.EventId, m.ConsumerGroup })
                .IsUnique()
                .HasDatabaseName("IX_InboxMessages_EventId_ConsumerGroup");

            configureEntity?.Invoke(b);
        });

        return modelBuilder;
    }
}
