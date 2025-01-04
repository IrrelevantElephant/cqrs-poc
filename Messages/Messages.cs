namespace Messages;

public record CacheUpdatedEvent;
public record CacheEmptyEvent;
public record CreateTodoCommand(string Id, string Name);
public record DeleteTodoCommand(string Id);
public record UpdateTodoCommand(string Id, string Name, bool Completed);
public record TodoCreatedEvent(string Id);
public record TodoDeletedEvent(string Id);
public record TodoUpdatedEvent(string Id);