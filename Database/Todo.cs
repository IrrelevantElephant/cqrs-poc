namespace Database;

public class Todo
{
    public required string Id { get; init; }
    public required string Name { get; set; }
    public bool Completed { get; set; }
}
