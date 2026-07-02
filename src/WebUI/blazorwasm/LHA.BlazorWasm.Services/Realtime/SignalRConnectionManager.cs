using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.HttpApi.Client.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using static LHA.Shared.Contracts.Realtime.SignalRHubMethodNames;

namespace LHA.BlazorWasm.Services.Realtime;

public sealed class SignalRConnectionManager(
    IOptions<HttpApiClientOptions> options,
    IAccessTokenProvider accessTokenProvider,
    ITokenRevocationHandler revocationHandler)
    : IAsyncDisposable
{
    private HubConnection? _connection;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_connection?.State is HubConnectionState.Connected
            or HubConnectionState.Connecting
            or HubConnectionState.Reconnecting)
        {
            return;
        }

        var token = await accessTokenProvider.GetTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(token))
        {
            return;
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        var hubUri = new Uri(new Uri(options.Value.BaseAddress), "hubs/notifications");
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUri, hubOptions =>
            {
                hubOptions.AccessTokenProvider = async () =>
                    await accessTokenProvider.GetTokenAsync(cancellationToken);
            })
            .WithAutomaticReconnect()
            .Build();

        _connection.On<object>(SecurityEvents.ForceLogout, async _ =>
        {
            await revocationHandler.HandleRevocationAsync(cancellationToken);
        });

        try
        {
            await _connection.StartAsync(cancellationToken);
        }
        catch
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is null)
        {
            return;
        }

        try
        {
            await _connection.StopAsync(cancellationToken);
        }
        catch
        {
        }

        await _connection.DisposeAsync();
        _connection = null;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
    }
}
