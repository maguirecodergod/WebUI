using LHA.EventBus;
using LHA.Identity.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace LHA.Notification.Consumer.Handlers;

public sealed class LoginSucceededEventHandler(ILogger<LoginSucceededEventHandler> logger)
    : IEventHandler<LoginSucceededEto>
{
    public Task HandleAsync(LoginSucceededEto @event, EventContext context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("🚀 [TEST] User {@UserName} (ID: {@UserId}) logged in successfully. Handled in Notification Consumer!", 
            @event.UserName, @event.UserId);
        
        return Task.CompletedTask;
    }
}
