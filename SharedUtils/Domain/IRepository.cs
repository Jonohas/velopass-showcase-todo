using System.Linq.Expressions;

namespace Shared.Domain;

public interface IRepository<TEntity, TKey> where
  TEntity : IEntity<TKey>
{
  Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken);
  Task<TEntity?> GetAsync(TKey id, CancellationToken cancellationToken);
  Task<TEntity?> GetNonTracking(TKey id, CancellationToken cancellationToken);
  Task<TEntity?> GetNonTrackingSplitQuery(TKey id, CancellationToken cancellationToken);

  Task<TProjection?> GetNonTrackingProjected<TProjection>(TKey id, Expression<Func<TEntity, TProjection>> projection,
    CancellationToken cancellationToken);

  Task<TProjection?> GetNonTrackingSplitQueryProjected<TProjection>(TKey id,
    Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken);

  /// <summary>
  /// Returns all entities, use sparingly because performance might be terrible
  /// </summary>
  /// <returns>All entities of a specific type</returns>
  Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);

  Task<List<TEntity>> GetAllNonTracking(CancellationToken cancellationToken);

  Task<List<TProjection>> GetAllProjected<TProjection>(Expression<Func<TEntity, TProjection>> projection,
    CancellationToken cancellationToken);

  Task<List<TProjection>> GetAllNonTrackingProjected<TProjection>(Expression<Func<TEntity, TProjection>> projection,
    CancellationToken cancellationToken);

  Task<List<TProjection>> GetAllNonTrackingSplitQueryProjected<TProjection>(
    Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken);

  Task SaveAsync(TEntity entity, CancellationToken token);
  void Save(TEntity entity);
  void Delete(TEntity entity);
}
