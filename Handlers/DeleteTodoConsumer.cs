using Database;
using MassTransit;
using Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Handlers;

internal class DeleteTodoConsumer(TodoContext todoContext, ILogger<DeleteTodoConsumer> logger) : IConsumer<DeleteTodoCommand>
{
    private ILogger<DeleteTodoConsumer> _logger = logger;
    public async Task Consume(ConsumeContext<DeleteTodoCommand> context)
    {
        var command = context.Message;
        var todoToDelete =
            await todoContext.Todos.FirstOrDefaultAsync(t => t.Id == command.Id, context.CancellationToken);

        if (todoToDelete == null)
        {
            _logger.LogError("Todo with id {TodoId} not found.", command.Id);
            return;
        }

        todoContext.Todos.Remove(todoToDelete);
        await todoContext.SaveChangesAsync(context.CancellationToken);
        await context.Publish(new TodoDeletedEvent(command.Id), context.CancellationToken);
    }
}