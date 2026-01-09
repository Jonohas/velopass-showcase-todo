using JV.ResultUtilities.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shared.Api;

namespace Shared.MultiTenancy.Middlewares;

/// <summary>
/// extracts tenant name from the "Tenant" header,
/// then uses ITenantService to fetch the tenant and set it in the ITenantContext
/// </summary>
/// <param name="RequestDelegate"></param>
public class TenantMiddleware<T>(RequestDelegate next)
  where T : ITenant
{
  public async Task InvokeAsync(HttpContext context, ITenantContext<T> tenantContext, IRequestTypeContext requestTypeContext, TenantResolutionOptions options)
  {
    requestTypeContext.SetRequestPath(context);

    // let openapi calls pass
    if (requestTypeContext.IsOpenApiRequest || !requestTypeContext.IsApiRequest)
    {
      await next(context);
      return;
    }

    string? tenantIdentifier = null;
    // header first
    if (context.Request.Headers.TryGetValue(options.HeaderName, out var headerVals))
    {
      tenantIdentifier = headerVals.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }
    // fallback to query if enabled
    if (tenantIdentifier is null && options.AllowQueryFallback && context.Request.Query.TryGetValue(options.QueryParameterName, out var queryVals))
    {
      tenantIdentifier = queryVals.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
    }

    if (string.IsNullOrWhiteSpace(tenantIdentifier))
    {
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
      await context.Response.WriteAsync("Tenant identifier missing.");
      return;
    }

    // Resolve ITenantService from the request scope
    var tenantService = context.RequestServices.GetRequiredService<ITenantService<T>>();
    var tenant = await tenantService.GetByNameAsync(tenantIdentifier, context.RequestAborted);
    if (tenant is null)
    {
      context.Response.StatusCode = StatusCodes.Status404NotFound;
      await context.Response.WriteAsync("Tenant not found.");
      return;
    }

    // run validators before committing tenant to context
    var validators = context.RequestServices.GetServices<ITenantValidator<T>>();
    foreach (var validator in validators)
    {
      try
      {
        var result = await validator.ValidateAsync(tenant, context.RequestAborted);
        result.ThrowIfFailure();
      }
      catch (Exception ex)
      {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Tenant validation exception: {ex.Message}");
        return;
      }
    }

    tenantContext.SetTenant(tenant);

    var listeners = context.RequestServices.GetServices<ITenantResolvedListener<T>>();
    foreach (var listener in listeners)
    {
      try { await listener.OnTenantResolvedAsync(tenant, context.RequestAborted); }
      catch { /* swallow listener exceptions to not block request */ }
    }

    await next(context);
  }
}
