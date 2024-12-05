using Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var databaseSettings = builder.Configuration.Get<DatabaseSettings>();

ArgumentNullException.ThrowIfNull(databaseSettings);

builder.Services.AddDbContext<TodoContext>(options => options.UseNpgsql(databaseSettings.ConnectionString));

var app = builder.Build();

// Seed data
var seedContext = app.Services.GetRequiredService<TodoContext>();
var dbTodos = await seedContext.Todos.ToListAsync();
if (dbTodos.Count == 0)
{
    await seedContext.AddRangeAsync(new List<Database.Todo>
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Ui");

app.MapGet("/api/todos", async (TodoContext context, CancellationToken cancellationToken) => await context.Todos.ToListAsync(cancellationToken));

app.MapPost("/api/todos", async (TodoContext context, CancellationToken cancellationToken, Todo newTodo) =>
{
    var todo = new Database.Todo
    {
        Id = newTodo.Id,
        Name = newTodo.Name,
        Completed = newTodo.Completed
    };

    await context.AddAsync(todo, cancellationToken);
    await context.SaveChangesAsync(cancellationToken);
    
    return TypedResults.Created();
});

app.MapPut("/api/todos/{id}", async (string id, Todo todo, TodoContext context, CancellationToken cancellationToken) =>
{
    var todoToUpdate = await context.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken: cancellationToken);

    if (todoToUpdate == null)
    {
        return Results.NotFound();
    }
    
    todoToUpdate.Completed = todo.Completed;
    todoToUpdate.Name = todo.Name;
    
    await context.SaveChangesAsync(cancellationToken);

    return Results.Ok();
});

app.MapDelete("/api/todos/{id}", async (string id, TodoContext context, CancellationToken cancellationToken) =>
{
    var todoToDelete = await context.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    if (todoToDelete == null)
    {
        return Results.NotFound();
    }

    context.Todos.Remove(todoToDelete);
    
    await context.SaveChangesAsync(cancellationToken);

    return Results.NoContent();
});

app.Run();

record Todo(string Id, string Name, bool Completed);