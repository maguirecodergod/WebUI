using Google.Protobuf;
using Grpc.Core;
using LHA.Grpc.Contracts.Common.V1;

namespace LHA.Grpc.Server.Streaming;

/// <summary>
/// Helpers for chunked binary streaming over gRPC.
/// Uses <see cref="DataChunk"/> from the common proto contracts.
/// </summary>
public static class ChunkedStreamHelper
{
    public const int DefaultChunkSize = 64 * 1024; // 64 KB

    /// <summary>
    /// Writes <paramref name="data"/> to the server stream as a series of <see cref="DataChunk"/> messages.
    /// Uses zero-copy <see cref="UnsafeByteOperations.UnsafeWrap"/> — the caller must keep the memory alive
    /// until this method returns.
    /// </summary>
    public static async Task WriteChunkedAsync(
        IServerStreamWriter<DataChunk> stream,
        ReadOnlyMemory<byte> data,
        int chunkSize = DefaultChunkSize,
        CancellationToken ct = default)
    {
        var totalSize = data.Length;
        var offset = 0;

        while (offset < totalSize)
        {
            ct.ThrowIfCancellationRequested();
            var size = Math.Min(chunkSize, totalSize - offset);
            var chunk = new DataChunk
            {
                Data = UnsafeByteOperations.UnsafeWrap(data.Slice(offset, size)),
                Offset = offset,
                TotalSize = totalSize,
                IsLast = offset + size >= totalSize,
            };
            await stream.WriteAsync(chunk, ct);
            offset += size;
        }
    }

    /// <summary>
    /// Reads all <see cref="DataChunk"/> messages from <paramref name="stream"/> and assembles them
    /// into a contiguous byte array.
    /// </summary>
    public static async Task<byte[]> ReadChunkedAsync(
        IAsyncStreamReader<DataChunk> stream,
        long? expectedTotalSize = null,
        CancellationToken ct = default)
    {
        var capacity = expectedTotalSize.HasValue ? (int)expectedTotalSize.Value : 256 * 1024;
        using var ms = new MemoryStream(capacity);

        await foreach (var chunk in stream.ReadAllAsync(ct))
        {
            chunk.Data.WriteTo(ms);
            if (chunk.IsLast) break;
        }

        return ms.ToArray();
    }
}
