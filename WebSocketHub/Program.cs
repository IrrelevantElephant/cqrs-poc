using Microsoft.AspNetCore.SignalR;
using Shared;
using WebSocketHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Ui",
        policy =>
        {
            policy.WithOrigins("http://localhost:8000");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowCredentials();
        });
});

builder.Services.AddSignalR();

builder.ConfigureOpenTelemetry("web_socket_hub", "1.0");

var appSettings = builder.Configuration.Get<AppSettings>();
ArgumentNullException.ThrowIfNull(appSettings);
builder.Services.ConfigureMassTransit(appSettings.MassTransitConfig, configurator =>
{
    configurator.AddConsumer<CacheUpdatedHandler>();
});

var app = builder.Build();

app.MapHub<NotificationHub>("/hub");

app.UseCors("Ui");

app.Run();

public class NotificationHub : Hub
{
    public async Task Send(string message) => await Clients.All.SendAsync("messageReceived", message);
}