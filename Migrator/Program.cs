using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();

optionsBuilder.UseNpgsql(connectionString);
var context = new TodoContext(optionsBuilder.Options);

await context.Database.MigrateAsync();

public class TodoContextFactory : IDesignTimeDbContextFactory<TodoContext>
{
    public TodoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TodoContext>();
        
        var connectionString = args[0];
        
        optionsBuilder.UseNpgsql(connectionString);
        return new TodoContext(optionsBuilder.Options);
    }
}