using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Api.Tests.Database;

/// <summary>
/// Helper class for database operations in tests
/// </summary>
public class DatabaseTestHelper<TDbContext> where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseTestHelper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Executes an action with a database context
    /// </summary>
    public async Task ExecuteInContextAsync(Func<TDbContext, Task> action)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await action(dbContext);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Executes a function with a database context and returns the result
    /// </summary>
    public async Task<T> ExecuteInContextAsync<T>(Func<TDbContext, Task<T>> func)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await func(dbContext);
    }

    /// <summary>
    /// Adds an entity to the database
    /// </summary>
    public async Task<T> AddEntityAsync<T>(T entity) where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var entry = await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Adds multiple entities to the database
    /// </summary>
    public async Task AddEntitiesAsync<T>(params T[] entities) where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Set<T>().AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets an entity by ID
    /// </summary>
    public async Task<T?> GetEntityByIdAsync<T>(object id) where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await dbContext.Set<T>().FindAsync(id);
    }

    /// <summary>
    /// Gets all entities of a type
    /// </summary>
    public async Task<List<T>> GetAllEntitiesAsync<T>() where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await dbContext.Set<T>().ToListAsync();
    }

    /// <summary>
    /// Clears all data from a specific table
    /// </summary>
    public async Task ClearTableAsync<T>() where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        dbContext.Set<T>().RemoveRange(dbContext.Set<T>());
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if any entities exist
    /// </summary>
    public async Task<bool> AnyAsync<T>() where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await dbContext.Set<T>().AnyAsync();
    }

    /// <summary>
    /// Counts entities
    /// </summary>
    public async Task<int> CountAsync<T>() where T : class
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        return await dbContext.Set<T>().CountAsync();
    }


    /// <summary>
    /// Ensures the database is created
    /// </summary>
    public async Task EnsureCreatedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Ensures the database is deleted
    /// </summary>
    public async Task EnsureDeletedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
    }

}
