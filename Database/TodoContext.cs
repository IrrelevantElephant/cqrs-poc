using Microsoft.EntityFrameworkCore;

namespace Database;

public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }
}