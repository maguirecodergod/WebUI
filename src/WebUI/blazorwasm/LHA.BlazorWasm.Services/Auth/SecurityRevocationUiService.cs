using LHA.BlazorWasm.Services.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LHA.BlazorWasm.Services.Auth;

public sealed class SecurityRevocationUiService(
    ILocalStorageService localStorage,
    AuthTokenCache tokenCache,
    NavigationManager navigationManager,
    IServiceProvider serviceProvider)
    : ISecurityRevocationUiService
{
    private const int GracePeriodSeconds = 5;

    private bool _revocationStarted;
    private bool _logoutExecuted;
    private CancellationTokenSource? _countdownCts;
    private Task? _countdownTask;

    public SecurityRevocationState State { get; } = new();

    public Task RequestGracefulRevocationAsync(CancellationToken cancellationToken = default)
    {
        if (_revocationStarted)
        {
            return _countdownTask ?? Task.CompletedTask;
        }

        _revocationStarted = true;
        _countdownCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        State.Show(GracePeriodSeconds);
        _countdownTask = RunRevocationAsync(_countdownCts.Token);
        return _countdownTask;
    }

    public async Task ConfirmLogoutAsync(CancellationToken cancellationToken = default)
    {
        if (_logoutExecuted)
        {
            return;
        }

        if (_countdownCts is not null)
        {
            await _countdownCts.CancelAsync();
        }

        if (_countdownTask is not null)
        {
            try
            {
                await _countdownTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        await ExecuteLogoutAsync(cancellationToken);
    }

    private async Task RunRevocationAsync(CancellationToken cancellationToken)
    {
        try
        {
            await StopSignalRAsync(cancellationToken);

            for (var remaining = GracePeriodSeconds; remaining > 0; remaining--)
            {
                State.UpdateRemainingSeconds(remaining);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

            await ExecuteLogoutAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private async Task ExecuteLogoutAsync(CancellationToken cancellationToken)
    {
        if (_logoutExecuted)
        {
            return;
        }

        _logoutExecuted = true;
        State.Hide();

        await StopSignalRAsync(cancellationToken);
        tokenCache.Clear();
        await localStorage.ClearAsync();
        navigationManager.NavigateTo("/login?reason=permissions_changed", forceLoad: true);
    }

    private Task StopSignalRAsync(CancellationToken cancellationToken)
    {
        var signalRConnectionManager = serviceProvider.GetService<Realtime.SignalRConnectionManager>();
        return signalRConnectionManager?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }
}
