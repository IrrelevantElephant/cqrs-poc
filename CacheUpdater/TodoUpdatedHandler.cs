using System.Text.Json;
using Database;
using MassTransit;
using Messages;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CacheUpdater;

public class TodoUpdatedHandler : IConsumer<TodoUpdatedEvent>
{
    private readonly IDatabase _cache;
    private readonly TodoContext _todoContext;

    public TodoUpdatedHandler(IDatabase cache, TodoContext todoContext)
    {
        _cache = cache;
        _todoContext = todoContext;
    }

    public async Task Consume(ConsumeContext<TodoUpdatedEvent> context)
    {
        var todos = await _todoContext.Todos.ToListAsync(context.CancellationToken);
        var todoJson = JsonSerializer.Serialize(todos);
        await _cache.StringSetAsync("GET", todoJson);
        await context.Publish(new CacheUpdatedEvent());
    }
}