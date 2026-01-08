// ServiceCruiser is our proprietary software and all source code, databases, functionality, software, website designs, audio, video, text, photographs, graphics (collectively referred to as the ‘Content’) and all intellectual property rights, including all copyright, all trademarks, all logos and all know-how vested therein or related thereto  (collectively referred to as the ‘IPR’) are owned, licenced or controlled by ESAS 3Services NV (or any of its affiliates or subsidiaries), excluded is Content or IPR provided and owned by third parties. All Content and IPR are protected by copyright and trademark laws and various other intellectual property rights legislation and/or other European Union and/or Belgian legislation, including unfair commercial practices legislation.

using Microsoft.Extensions.Caching.Hybrid;
using Shared.FeatureFlags.Entities;
using Shared.FeatureFlags.Repositories;

namespace Shared.FeatureFlags.Services;

public interface IFeatureFlagService
{
  Task AddFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);
  Task<Dictionary<string, bool>> GetEffectiveFlagsAsync(Guid tenantId, CancellationToken cancellationToken);
  Task<bool?> GetEffectiveFlagAsync(Guid tenantId, string code, CancellationToken cancellationToken);
}

public class FeatureFlagService : IFeatureFlagService
{
  private readonly IFeatureFlagRepository _repository;
  private readonly HybridCache _cache;
  private readonly FeatureFlagOptions _options;

  public FeatureFlagService(IFeatureFlagRepository repository, HybridCache cache, FeatureFlagOptions options)
  {
    _repository = repository;
    _cache = cache;
    _options = options;
  }

  public Task AddFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken)
  {
    return _repository.SaveAsync(featureFlag, cancellationToken);
  }

  // TODO@JOREN: optimize cache factory with state
  public async Task<Dictionary<string, bool>> GetEffectiveFlagsAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var key = FeatureFlagCacheKeys.EffectiveByTenant(tenantId);

    return await _cache.GetOrCreateAsync(key,
             async ctoken => await _repository.GetEffectiveFlagsAsync(tenantId, ctoken),
             options: new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(_options.CacheSeconds) },
             cancellationToken: cancellationToken)
           ?? new();
  }

  public async Task<bool?> GetEffectiveFlagAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    var key = FeatureFlagCacheKeys.Single(tenantId, code);

    return await _cache.GetOrCreateAsync(key,
             async ctoken => await _repository.GetEffectiveFlagAsync(tenantId, code, ctoken),
             options: new HybridCacheEntryOptions { Expiration = TimeSpan.FromSeconds(_options.CacheSeconds) },
             cancellationToken: cancellationToken)
           ?? new();
  }
}
