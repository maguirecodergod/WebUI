namespace LHA.BlazorWasm.Services.Toast;

/// <summary>
/// Structural envelope housing individual rendering state for a Toast message.
/// </summary>
public class ToastMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string? Title { get; init; }
    public string Message { get; init; } = string.Empty;
    public CToastLevel Level { get; init; } = CToastLevel.Info;
    public int Duration { get; init; } = 3000;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
