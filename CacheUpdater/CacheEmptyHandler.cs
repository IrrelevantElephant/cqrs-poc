using Database;
using MassTransit;
using Messages;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheUpdater;

public class CacheEmptyHandler(IDatabase cache, TodoContext todoContext) : IConsumer<CacheEmptyEvent>
{
    public async Task Consume(ConsumeContext<CacheEmptyEvent> context)
    {
        var todos = await todoContext.Todos.ToListAsync(context.CancellationToken);
        var todoJson = JsonSerializer.Serialize(todos);
        await cache.StringSetAsync("GET", todoJson);
        await context.Publish(new CacheUpdatedEvent());
    }
}