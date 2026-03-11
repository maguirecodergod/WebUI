using System;

namespace LHA.BlazorWasm.Services.Toast;

/// <summary>
/// Structural envelope housing individual rendering state for a Toast message.
/// </summary>
public class ToastMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Message { get; init; } = string.Empty;
    public ToastLevel Level { get; init; } = ToastLevel.Info;
    public int Duration { get; init; } = 3000;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
