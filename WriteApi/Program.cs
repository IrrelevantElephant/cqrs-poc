using MassTransit;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Shared;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.Get<AppSettings>();
builder.ConfigureOpenTelemetry("write_api", "1.0");

builder.Services.ConfigureMassTransit(appSettings!.MassTransitConfig);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/todos", async ([FromServices]IBus bus, CancellationToken cancellationToken, Todo newTodo) =>
{
    await bus.Publish(new CreateTodoCommand(newTodo.Id, newTodo.Name), cancellationToken);
    return TypedResults.Created();
});

app.MapPut("/api/todos/{id}", async (string id, Todo todo, [FromServices]IBus bus, CancellationToken cancellationToken) =>
{
    await bus.Publish(new UpdateTodoCommand(id, todo.Name, todo.Completed), cancellationToken);
    return Results.Ok();
});

app.MapDelete("/api/todos/{id}",
    async (string id, [FromServices]IBus bus, CancellationToken cancellationToken) =>
    {
        await bus.Publish(new DeleteTodoCommand(id), cancellationToken);
        return Results.NoContent();
    });

app.Run();

internal record Todo(string Id, string Name, bool Completed);