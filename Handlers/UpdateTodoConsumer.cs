using Database;
using MassTransit;
using Messages;
using Microsoft.EntityFrameworkCore;

namespace Handlers;

internal class UpdateTodoConsumer(TodoContext todoContext) : IConsumer<UpdateTodoCommand>
{
    public async Task Consume(ConsumeContext<UpdateTodoCommand> context)
    {
        var command = context.Message;
        var todoToUpdate =
            await todoContext.Todos.FirstOrDefaultAsync(t => t.Id == command.Id,
                cancellationToken: context.CancellationToken);

        if (todoToUpdate == null)
        {
            return;
        }

        todoToUpdate.Completed = command.Completed;
        todoToUpdate.Name = command.Name;

        await todoContext.SaveChangesAsync(context.CancellationToken);
        await context.Publish(new TodoUpdatedEvent(command.Id), context.CancellationToken);
    }
}