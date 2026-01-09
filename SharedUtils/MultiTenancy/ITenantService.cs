namespace Shared.MultiTenancy;

public interface ITenantService<T>
  where T: ITenant
{
  Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken);
  Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
