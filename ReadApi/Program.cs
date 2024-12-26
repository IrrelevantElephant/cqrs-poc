using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shared;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry("read_api", "1.0");

builder.Services.AddOpenApi();

var cacheSettings = builder.Configuration.Get<CacheSettings>();
ArgumentNullException.ThrowIfNull(cacheSettings);
var redis = ConnectionMultiplexer.Connect(cacheSettings.CacheConnectionString);
var database = redis.GetDatabase();
builder.Services.AddSingleton(database);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/todos",
    async ([FromServices]IDatabase cache, CancellationToken cancellationToken) =>
    {
        var cachedTodos = await cache.StringGetAsync("GET");
        var todos = JsonSerializer.Deserialize<List<Todo>>(cachedTodos);
        return todos;
    });

app.Run();

internal record Todo(string Id, string Name, bool Completed);