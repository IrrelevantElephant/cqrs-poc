using MassTransit;
using Messages;
using Microsoft.AspNetCore.SignalR;

namespace WebSocketHub;

public class CacheUpdatedHandler : IConsumer<CacheUpdatedEvent>
{
    private IHubContext<NotificationHub> _hubContext;

    public CacheUpdatedHandler(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<CacheUpdatedEvent> context)
    {
        await _hubContext.Clients.All.SendAsync("CacheUpdated", context.Message);
    }
}