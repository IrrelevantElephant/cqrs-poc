using Database;
using MassTransit;
using Messages;

namespace Handlers;

internal class CreateTodoConsumer(TodoContext todoContext) : IConsumer<CreateTodoCommand>
{
    public async Task Consume(ConsumeContext<CreateTodoCommand> context)
    {
        var command = context.Message;
        var todo = new Todo
        {
            Id = command.Id,
            Name = command.Name
        };

        await todoContext.Todos.AddAsync(todo, context.CancellationToken);
        await todoContext.SaveChangesAsync(context.CancellationToken);
        await context.Publish(new TodoCreatedEvent(command.Id), context.CancellationToken);
    }
}

