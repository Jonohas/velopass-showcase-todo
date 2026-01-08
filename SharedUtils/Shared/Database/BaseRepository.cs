using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace Shared.Database;

public interface IBaseRepository<TEntity, TKey>
  where TEntity : Entity<TKey>
{
  Task SaveAsync(TEntity entity, CancellationToken token);
  void Save(TEntity entity);
  void Delete(TEntity entity);
}

public abstract class BaseRepository<TEntity, TKey, TContext> : IBaseRepository<TEntity, TKey>
where TEntity : Entity<TKey>
where TContext : DbContext
{
  protected readonly TContext? DbContext;
  protected readonly IMultiTenantDbContextFactory<TContext>? DbContextFactory;

  /// <summary>
  /// Returns the active DbContext.
  /// For multi-tenant, a new one is created each call.
  /// For single-tenant, the same context is reused.
  /// </summary>
  protected TContext Context =>
    DbContext ?? DbContextFactory!.CreateDbContext();

  protected DbSet<TEntity> Set => Context.Set<TEntity>();

  protected BaseRepository(TContext dbContext)
  {
    DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
  }

  protected BaseRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory)
  {
    DbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
  }
  
  public async Task SaveAsync(TEntity entity, CancellationToken token)
  {
    if (!Set.Local.Contains(entity))
      await Set.AddAsync(entity, token);
  }

  public void Save(TEntity entity)
  {
    if (!Set.Local.Contains(entity))
      Set.Add(entity);
  }

  public void Delete(TEntity entity)
  {
    Set.Remove(entity);
  }
}