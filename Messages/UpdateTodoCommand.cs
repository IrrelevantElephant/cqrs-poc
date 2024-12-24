namespace Messages;

public record UpdateTodoCommand(string Id, string Name, bool Completed);
