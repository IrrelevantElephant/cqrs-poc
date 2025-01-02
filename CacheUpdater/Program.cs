using CacheUpdater;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using Shared;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

var databaseSettings = builder.Configuration.Get<DatabaseSettings>();
ArgumentNullException.ThrowIfNull(databaseSettings);
builder.Services.AddDbContext<TodoContext>(options => options.UseNpgsql(databaseSettings.ConnectionString));

var cacheSettings = builder.Configuration.Get<CacheSettings>();
ArgumentNullException.ThrowIfNull(cacheSettings);
var redis = ConnectionMultiplexer.Connect(cacheSettings.CacheConnectionString);
var database = redis.GetDatabase();
builder.Services.AddSingleton(database);

builder.ConfigureOpenTelemetry("cache_updater", "1.0", traceBuilder =>
{
    traceBuilder.AddRedisInstrumentation(redis);
});

var appSettings = builder.Configuration.Get<AppSettings>();
ArgumentNullException.ThrowIfNull(appSettings);
builder.Services.ConfigureMassTransit(appSettings.MassTransitConfig, configurator =>
{
    configurator.AddConsumer<TodoCreatedHandler>();
    configurator.AddConsumer<TodoDeletedHandler>();
    configurator.AddConsumer<TodoUpdatedHandler>();
});

var host = builder.Build();

host.Run();
