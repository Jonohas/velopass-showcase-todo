using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Api;
using Shared.Caching;
using Shared.Keycloak.User;
using Shared.MultiTenancy;
using Shared.Permissions;

namespace Shared.Keycloak;

public static class KeycloakInstaller
{
  public static IServiceCollection AddMultiTenantKeycloak<TTenant, TUser, TDbContext>(this IServiceCollection services,
    KeycloakConfiguration keycloakConfiguration, CacheConfiguration cacheConfiguration,
    IReadOnlyList<string> permissions,
    List<string>? nonApiPaths = null)
    where TTenant : ITenant
    where TUser : BaseUser
    where TDbContext : DbContext
  {
    // needed services
    services.AddRequestTypeContext(nonApiPaths);
    services.AddCaching(cacheConfiguration);
    services.AddPermissionRepositories<TDbContext>();

    // Register the custom authentication handler
    services.AddSingleton(keycloakConfiguration);
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddScheme<JwtBearerOptions, MultiTenantJwtBearerHandler<TTenant, TUser>>(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
          // Base configuration - will be overridden per tenant
          options.SaveToken = true;
          options.IncludeErrorDetails = true;
        });

    // Authorization policies
    services.AddAuthorization(options =>
    {
      foreach (var permission in permissions)
        options.AddPolicy(permission, policy =>
          policy.RequireClaim(Constants.PermissionClaimType, permission));

      options.AddPolicy(Constants.TenantPolicyName, policy =>
        policy.RequireAuthenticatedUser());
    });

    services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();

    return services;
  }
}
