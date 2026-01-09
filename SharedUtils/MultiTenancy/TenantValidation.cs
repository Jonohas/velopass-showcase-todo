using JV.ResultUtilities;
using JV.ResultUtilities.ValidationMessage;

namespace Shared.MultiTenancy;

/// <summary>
/// Validates a tenant instance before it is committed to the request context.
/// Return Result.Ok() if valid; Result.Fail(errorMessage) otherwise.
/// </summary>
public interface ITenantValidator<T> where T : ITenant
{
  Task<Result> ValidateAsync(T tenant, CancellationToken cancellationToken);
}

/// <summary>
/// Ensures the realm name is non-empty and contains only allowed characters (letters, digits, dash).
/// </summary>
public sealed class RealmNameFormatValidator<T> : ITenantValidator<T> where T : ITenant
{
  public Task<Result> ValidateAsync(T tenant, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(tenant.RealmName))
      return Task.FromResult(Result.Error(TenantValidationKeys.Tenant.RealmNameRequired));
    
    foreach (var c in tenant.RealmName)
    {
      if (!(char.IsLetterOrDigit(c) || c == '-'))
        return Task.FromResult(Result.Error(TenantValidationKeys.Tenant.RealmNameInvalidChar, c.ToString()));
    }
    return Task.FromResult(Result.Ok());
  }
}

/// <summary>
/// Ensures the connection string is not null/empty and contains a minimal required token (e.g. 'Server=' for PostgreSQL).
/// </summary>
public sealed class ConnectionStringBasicValidator<T> : ITenantValidator<T> where T : ITenant
{
  private readonly string _requiredToken;
  public ConnectionStringBasicValidator(string requiredToken = "Server=") => _requiredToken = requiredToken;
  public Task<Result> ValidateAsync(T tenant, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(tenant.ConnectionString))
      return Task.FromResult(Result.Error(TenantValidationKeys.Tenant.ConnectionStringRequired));
    
    if (!tenant.ConnectionString.Contains(_requiredToken, StringComparison.OrdinalIgnoreCase))
      return Task.FromResult(Result.Error(TenantValidationKeys.Tenant.ConnectionStringMissingToken, _requiredToken));
    
    return Task.FromResult(Result.Ok());
  }
}
