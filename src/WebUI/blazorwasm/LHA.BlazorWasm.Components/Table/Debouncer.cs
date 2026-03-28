namespace LHA.BlazorWasm.Components.Table;

/// <summary>Simple debouncer for UI inputs (search, filter).</summary>
public sealed class Debouncer : IDisposable
{
    private readonly int _delayMs;
    private CancellationTokenSource? _cts;

    public Debouncer(int delayMs) => _delayMs = delayMs;

    public async Task DebounceAsync(Func<Task> action)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        try
        {
            await Task.Delay(_delayMs, _cts.Token);
            await action();
        }
        catch (TaskCanceledException) { }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
