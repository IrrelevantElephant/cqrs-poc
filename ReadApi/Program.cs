using MassTransit;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Shared;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry("read_api", "1.0");

builder.Services.AddOpenApi();

var cacheSettings = builder.Configuration.Get<CacheSettings>();
ArgumentNullException.ThrowIfNull(cacheSettings);
var redis = ConnectionMultiplexer.Connect(cacheSettings.CacheConnectionString);
var database = redis.GetDatabase();
builder.Services.AddSingleton(database);

var appSettings = builder.Configuration.Get<AppSettings>();
builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/todos",
    async ([FromServices]IDatabase cache, [FromServices]IBus bus, CancellationToken cancellationToken) =>
    {
        var cachedTodos = await cache.StringGetAsync("GET");

        if (cachedTodos.IsNull)
        {
            await bus.Publish(new CacheEmptyEvent(), cancellationToken);
            return [];
        }

        var todos = JsonSerializer.Deserialize<List<Todo>>(cachedTodos);
        return todos;
    });

app.Run();

internal record Todo(string Id, string Name, bool Completed);