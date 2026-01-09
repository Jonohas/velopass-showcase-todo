namespace Shared.MultiTenancy;

/// <summary>
/// Options controlling how the tenant is resolved per request.
/// </summary>
public sealed class TenantResolutionOptions
{
  /// <summary>Name of the HTTP header used to identify the tenant (default: "Tenant").</summary>
  public string HeaderName { get; set; } = "Tenant";
  /// <summary>Whether a query string fallback is allowed when the header is absent.</summary>
  public bool AllowQueryFallback { get; set; } = false;
  /// <summary>Name of the query parameter to inspect when <see cref="AllowQueryFallback"/> is true (default: "tenant").</summary>
  public string QueryParameterName { get; set; } = "tenant";
}

/// <summary>
/// Implementations are invoked after a tenant has been resolved and set in the context.
/// Can be used to warm caches, enrich logs, record metrics, etc.
/// Exceptions should be handled internally; throwing will not block the request but is discouraged.
/// </summary>
/// <typeparam name="T">Concrete tenant type.</typeparam>
public interface ITenantResolvedListener<T> where T : ITenant
{
  /// <summary>Callback invoked once per request after tenant resolution.</summary>
  Task OnTenantResolvedAsync(T tenant, CancellationToken cancellationToken);
}
