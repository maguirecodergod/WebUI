using Grpc.Core;

namespace LHA.Grpc.Client.Streaming;

/// <summary>
/// Convenience extensions for consuming and producing gRPC streams.
/// </summary>
public static class StreamingExtensions
{
    /// <summary>Writes all items from an <see cref="IAsyncEnumerable{T}"/> and completes the stream.</summary>
    public static async Task WriteAllAsync<T>(
        this IClientStreamWriter<T> writer,
        IAsyncEnumerable<T> items,
        CancellationToken ct = default)
    {
        await foreach (var item in items.WithCancellation(ct))
            await writer.WriteAsync(item);
        await writer.CompleteAsync();
    }

    /// <summary>Writes all items from an <see cref="IEnumerable{T}"/> and completes the stream.</summary>
    public static async Task WriteAllAsync<T>(
        this IClientStreamWriter<T> writer,
        IEnumerable<T> items,
        CancellationToken ct = default)
    {
        foreach (var item in items)
        {
            ct.ThrowIfCancellationRequested();
            await writer.WriteAsync(item);
        }
        await writer.CompleteAsync();
    }

    /// <summary>Reads all messages from a server stream into a list.</summary>
    public static async Task<List<T>> ToListAsync<T>(
        this IAsyncStreamReader<T> reader,
        CancellationToken ct = default)
    {
        var list = new List<T>();
        await foreach (var item in reader.ReadAllAsync(ct))
            list.Add(item);
        return list;
    }

    /// <summary>Invokes <paramref name="handler"/> for each message in a server stream.</summary>
    public static async Task ForEachAsync<T>(
        this IAsyncStreamReader<T> reader,
        Func<T, Task> handler,
        CancellationToken ct = default)
    {
        await foreach (var item in reader.ReadAllAsync(ct))
            await handler(item);
    }

    /// <summary>Invokes <paramref name="handler"/> for each message with cancellation support.</summary>
    public static async Task ForEachAsync<T>(
        this IAsyncStreamReader<T> reader,
        Func<T, CancellationToken, Task> handler,
        CancellationToken ct = default)
    {
        await foreach (var item in reader.ReadAllAsync(ct))
            await handler(item, ct);
    }
}
