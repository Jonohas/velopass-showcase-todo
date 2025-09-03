using System.Linq.Expressions;

namespace Shared;

public interface IRepository<TEntity, TKey> where
    TEntity : Entity<TKey>
{
    Task<TEntity?> Get(TKey id, CancellationToken cancellationToken);

    Task<TEntity?> GetNonTracking(TKey id, CancellationToken cancellationToken);
    
    Task<List<TEntity>> GetAll(CancellationToken cancellationToken);
    Task SaveAsync(TEntity entity, CancellationToken token);
    void Save(TEntity entity);
    void Delete(TEntity entity);
}
