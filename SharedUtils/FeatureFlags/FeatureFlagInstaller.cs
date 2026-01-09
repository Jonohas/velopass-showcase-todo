using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.FeatureFlags.Authorization;
using Shared.FeatureFlags.Repositories;
using Shared.FeatureFlags.Services;

namespace Shared.FeatureFlags;

public static class FeatureFlagInstaller
{
  /// <summary>
  /// Adds per-tenant feature flagging. Opt-in by calling this and ensuring your tenant DbContext implements IFeatureFlagDbContext.
  /// </summary>
  public static IServiceCollection AddFeatureFlagAuthorization(this IServiceCollection services, FeatureFlagOptions? options = null)
  {
    options ??= new FeatureFlagOptions();
    services.AddSingleton(options);

    services.AddScoped<IFeatureFlagChecker, FeatureFlagChecker>();
    services.AddScoped<IAuthorizationHandler, FeatureFlagAuthorizationHandler>();
    
    return services;
  }

  public static IServiceCollection AddFeatureFlagDomain<TDbContext>(this IServiceCollection services,
    FeatureFlagOptions? options = null)
    where TDbContext : DbContext, IFeatureFlagDbContext
  {
    options ??= new FeatureFlagOptions();
    services.AddSingleton(options);

    services.AddScoped<IFeatureFlagRepository, FeatureFlagRepository<TDbContext>>();
    services.AddScoped<IFeatureFlagService, FeatureFlagService>();
    
    return services;
  }
}

