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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("Ui");

var todos = new List<Todo>
{
    new Todo("todo-0", "Eat", true),
    new Todo("todo-1", "Sleep", false),
    new Todo("todo-2", "Repeat", false),
};

app.MapGet("/api/todos", () => todos).WithName("GetTodos");

app.MapPost("/api/todos", (Todo newTodo) =>
{
    todos.Add(newTodo);
    return TypedResults.Created();
});

app.MapPut("/api/todos/{id}", (string id, Todo todo) =>
{
    todos = todos.Where(t => t.Id != id).Append(todo).ToList();

    return TypedResults.Ok();
});

app.MapDelete("/api/todos/{id}", (string id) =>
{
    todos = todos.Where(t => t.Id != id).ToList();

    return TypedResults.NoContent();
});

app.Run();

record Todo(string Id, string Name, bool Completed);