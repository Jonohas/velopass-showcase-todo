using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Shared.Caching;

public static class CacheInstaller
{
  public static IServiceCollection AddCaching(this IServiceCollection services, CacheConfiguration config)
  {
    var fusionCacheBuilder = services.AddFusionCache()
      .WithDefaultEntryOptions(new FusionCacheEntryOptions
      {
        // cache duration
        Duration = CacheTimes.Short,

        // FAIL-SAFE OPTIONS
        IsFailSafeEnabled = true,
        // we specify for how long a value should be usable, even after its logical expiration
        FailSafeMaxDuration = CacheTimes.Medium,
        // we also specify for how long an expired value used because of a fail-safe activation should be
        // considered temporarily non-expired, to avoid going to check the database for every
        // consecutive request of an expired value
        FailSafeThrottleDuration = TimeSpan.FromSeconds(30)
      })
      .WithSerializer(new FusionCacheSystemTextJsonSerializer())
      .WithMemoryCache(new MemoryCache(new MemoryCacheOptions()))
      .AsHybridCache()
      .WithoutDistributedCache() // we do not use distributed cache
      .WithoutBackplane();

    if (!config.EnableLogger)
      fusionCacheBuilder.WithoutLogger(); // enabling the logger adds significant overhead

    return services;
  }
}
