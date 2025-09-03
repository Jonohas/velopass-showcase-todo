using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Shared;

public abstract class EFRepository<TEntity, TKey, TContext> : IRepository<TEntity, TKey>
  where TEntity : Entity<TKey>
  where TKey : IComparable<TKey>
  where TContext : DbContext
{
  protected readonly TContext Context;

  protected EFRepository(TContext dbContext)
  {
    Context = dbContext;
  }

  protected DbSet<TEntity> Set => Context.Set<TEntity>();

  public Task<TEntity?> Get(TKey id, CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(Get))
      .IgnoreAutoIncludes()
      .FirstOrDefaultAsync(a => a.Id.Equals(id), cancellationToken: cancellationToken);
  }

  public Task<TEntity?> GetNonTracking(TKey id, CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetNonTracking))
      .AsNoTracking()
      .IgnoreAutoIncludes()
      .FirstOrDefaultAsync(a => a.Id.Equals(id), cancellationToken);
  }
  
  public Task<List<TEntity>> GetAll(CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAll))
      .IgnoreAutoIncludes()
      .ToListAsync(cancellationToken);
  }

  public virtual async Task SaveAsync(TEntity entity, CancellationToken token)
  {
    if (!Set.Local.Contains(entity))
      await Set.AddAsync(entity, token);
  }

  public virtual void Save(TEntity entity)
  {
    if (!Set.Local.Contains(entity))
      Set.Add(entity);
  }

  public void Delete(TEntity entity)
  {
    Set.Remove(entity);
  }
}