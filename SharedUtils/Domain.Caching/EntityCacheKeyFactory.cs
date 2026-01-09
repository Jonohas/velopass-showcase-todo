namespace Shared.Domain.Caching;

public static class EntityCacheKeyFactory<T>
where T : Entity<Guid>
{
  public static string Base => $"{typeof(T).Name}.Base";
  public static string GetById(Guid id) => $"{typeof(T).Name}.GetById.{id}";
  public static string Exists(Guid id) => $"{typeof(T).Name}.Exists.{id}";
}
