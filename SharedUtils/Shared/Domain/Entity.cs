namespace Shared.Domain;

public interface IEntity<TKey>
{
  TKey Id { get; }
}

public abstract class Entity<TKey> : IEntity<TKey>
{
  public virtual TKey Id { get; protected set; } = default!;
}
