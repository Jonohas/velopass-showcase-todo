// may be unused now

using Shared.Domain;

namespace Shared.MultiTenancy;

public interface ITenant : IEntity<Guid>
{
  new Guid Id { get; }
  string Name { get; }
  string ConnectionString { get; }
  string RealmName { get; }
}

public interface ITenantContext<T>
  where T : ITenant
{
  T Tenant { get; }
  bool HasTenant { get; }
  bool TryGetTenant(out T tenant);
  void SetTenant(T tenant);
  bool TrySetTenant(T tenant);
  T EnsureTenant();
}

public class TenantContext<T> : ITenantContext<T>
  where T : ITenant
{
  private T? _tenant;

  public TenantContext() {}

  public T Tenant => _tenant ?? throw new InvalidOperationException("Tenant has not been set for the current scope");
  public bool HasTenant => _tenant is not null;
  public bool TryGetTenant(out T tenant)
  {
    if (_tenant is not null)
    {
      tenant = _tenant;
      return true;
    }
    tenant = default!;
    return false;
  }

  public void SetTenant(T tenant)
  {
    if (tenant is null) throw new ArgumentNullException(nameof(tenant));
    if (_tenant is not null) throw new InvalidOperationException("Tenant already set; resetting is not allowed within the same scope");
    _tenant = tenant;
  }

  public bool TrySetTenant(T tenant)
  {
    if (tenant is null) throw new ArgumentNullException(nameof(tenant));
    if (_tenant is not null) return false;
    _tenant = tenant;
    return true;
  }

  public T EnsureTenant() => Tenant;
}
