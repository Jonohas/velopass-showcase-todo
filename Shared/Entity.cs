

public abstract class Entity<TKey>
{
    public virtual TKey Id { get; protected set; } = default!;
    public virtual DateTimeOffset CreatedAt { get; protected set; } = DateTime.UtcNow;
}