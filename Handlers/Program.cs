using Database;
using Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

var builder = Host.CreateApplicationBuilder(args);
builder.ConfigureOpenTelemetry("handlers", "1.0");

var databaseSettings = builder.Configuration.Get<DatabaseSettings>();
ArgumentNullException.ThrowIfNull(databaseSettings);
builder.Services.AddDbContext<TodoContext>(options => options.UseNpgsql(databaseSettings.ConnectionString));

var appSettings = builder.Configuration.Get<AppSettings>();
ArgumentNullException.ThrowIfNull(appSettings);
builder.Services.ConfigureMassTransit(appSettings.MassTransitConfig, configurator =>
{
    configurator.AddConsumer<CreateTodoConsumer>();
    configurator.AddConsumer<UpdateTodoConsumer>();
    configurator.AddConsumer<DeleteTodoConsumer>();
});

var host = builder.Build();

// Seed data
var seedContext = host.Services.GetRequiredService<TodoContext>();
var dbTodos = await seedContext.Todos.ToListAsync();
if (dbTodos.Count == 0)
{
    await seedContext.AddRangeAsync(new List<Todo>
    {
        new()
        {
            Id = "todo-0",
            Name = "Eat",
            Completed = true
        },
        new()
        {
            Id = "todo-1",
            Name = "Sleep",
            Completed = false
        },
        new()
        {
            Id = "todo-2",
            Name = "Repeat",
            Completed = false
        }
    });

    await seedContext.SaveChangesAsync();
}

host.Run();