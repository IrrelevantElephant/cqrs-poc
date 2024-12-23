using Database;
using Microsoft.EntityFrameworkCore;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry("read_api", "1.0");

builder.Services.AddOpenApi();

var databaseSettings = builder.Configuration.Get<DatabaseSettings>();

ArgumentNullException.ThrowIfNull(databaseSettings);

builder.Services.AddDbContext<TodoContext>(options => options.UseNpgsql(databaseSettings.ConnectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();