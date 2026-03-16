using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LHA.BackgroundWorker;

/// <summary>
/// Bridges <see cref="IBackgroundWorkerManager"/> into the .NET Generic Host lifecycle.
/// Automatically starts/stops all registered workers.
/// </summary>
internal sealed class BackgroundWorkerHostedService : IHostedService
{
    private readonly IBackgroundWorkerManager _manager;
    private readonly BackgroundWorkerOptions _options;

    public BackgroundWorkerHostedService(
        IBackgroundWorkerManager manager,
        IOptions<BackgroundWorkerOptions> options)
    {
        _manager = manager;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.IsEnabled) return;
        await _manager.StartAllAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (!_options.IsEnabled) return;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(_options.ShutdownTimeout);
        await _manager.StopAllAsync(cts.Token);
    }
}
