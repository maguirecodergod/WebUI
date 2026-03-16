using LHA.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LHA.EventBus.Kafka;

/// <summary>
/// Background service that periodically processes pending outbox messages
/// and forwards them to Kafka.
/// </summary>
internal sealed class KafkaOutboxProcessorBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<EventBusOptions> options,
    ILogger<KafkaOutboxProcessorBackgroundService> logger) : BackgroundService
{
    private readonly EventBusOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.EnableOutbox)
        {
            logger.LogInformation("Kafka outbox processor is disabled (EnableOutbox=false). Stopping.");
            return;
        }

        logger.LogInformation("Kafka outbox processor starting. Polling interval: {Interval}s",
            _options.OutboxPollingInterval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                using var uow = uowManager.Begin();

                var processor = scope.ServiceProvider.GetRequiredService<KafkaOutboxProcessor>();
                await processor.ProcessBatchAsync(stoppingToken);

                await uow.CompleteAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Kafka outbox batch.");
            }

            await Task.Delay(_options.OutboxPollingInterval, stoppingToken);
        }
    }
}
