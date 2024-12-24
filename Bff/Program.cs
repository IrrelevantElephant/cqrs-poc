using Bff;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry("bff", "1.0");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Ui",
        policy =>
        {
            policy.WithOrigins("http://localhost:8000");
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

builder.Services.AddOpenApi();

var proxySettings = builder.Configuration.Get<ProxySettings>();
ArgumentNullException.ThrowIfNull(proxySettings);
var (routes, clusters) = YarpConfiguration.GetYarpConfigurations(proxySettings);
builder.Services.AddReverseProxy().LoadFromMemory(routes, clusters);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Ui");

app.MapReverseProxy();

app.Run();
