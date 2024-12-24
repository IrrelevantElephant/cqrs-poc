using Database;
using MassTransit;
using Messages;
using Microsoft.EntityFrameworkCore;

namespace Handlers;

internal class DeleteTodoConsumer(TodoContext todoContext) : IConsumer<DeleteTodoCommand>
{
    public async Task Consume(ConsumeContext<DeleteTodoCommand> context)
    {
        var command = context.Message;
        var todoToDelete =
            await todoContext.Todos.FirstOrDefaultAsync(t => t.Id == command.Id, context.CancellationToken);

        if (todoToDelete == null)
        {
            return;
        }

        todoContext.Todos.Remove(todoToDelete);
        await todoContext.SaveChangesAsync(context.CancellationToken);
        await context.Publish(new TodoDeletedEvent(command.Id), context.CancellationToken);
    }
}