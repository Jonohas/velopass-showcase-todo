using Domain;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class TodoDbContext : DbContext
{
    public const string DefaultSchema = "todos";
    public const string MigrationsSchema = "migrations";
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Todo> TodoItems { get; set; }
    
    /// <summary>
    /// Configure the model that was discovered by convention from the entity types exposed in <see cref="DbSet{TEntity}"/> properties on this context.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
    }

}