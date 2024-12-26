using System.Text.Json;
using Database;
using MassTransit;
using Messages;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CacheUpdater;

public class TodoCreatedHandler : IConsumer<TodoCreatedEvent>
{
    private readonly IDatabase _cache;
    private readonly TodoContext _todoContext;

    public TodoCreatedHandler(IDatabase cache, TodoContext todoContext)
    {
        _cache = cache;
        _todoContext = todoContext;
    }

    public async Task Consume(ConsumeContext<TodoCreatedEvent> context)
    {
        var todos = await _todoContext.Todos.ToListAsync(context.CancellationToken);
        var todoJson = JsonSerializer.Serialize(todos);
        await _cache.StringSetAsync("GET", todoJson);
    }
}