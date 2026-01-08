// ServiceCruiser is our proprietary software and all source code, databases, functionality, software, website designs, audio, video, text, photographs, graphics (collectively referred to as the ‘Content’) and all intellectual property rights, including all copyright, all trademarks, all logos and all know-how vested therein or related thereto  (collectively referred to as the ‘IPR’) are owned, licenced or controlled by ESAS 3Services NV (or any of its affiliates or subsidiaries), excluded is Content or IPR provided and owned by third parties. All Content and IPR are protected by copyright and trademark laws and various other intellectual property rights legislation and/or other European Union and/or Belgian legislation, including unfair commercial practices legislation.

using Microsoft.Extensions.Caching.Hybrid;
using Shared.FeatureFlags.Repositories;

namespace Shared.FeatureFlags.Services;

public interface IFeatureFlagTenantOverrideService
{
  Task UpdateFeatureFlagAsync(Guid tenantId, string code, bool enabled, CancellationToken cancellationToken);

}

public class FeatureFlagTenantOverrideService : IFeatureFlagTenantOverrideService
{
  private readonly IFeatureFlagRepository _repository;
  private readonly HybridCache _cache;
  private readonly FeatureFlagOptions _options;

  public FeatureFlagTenantOverrideService(IFeatureFlagRepository repository, HybridCache cache, FeatureFlagOptions options)
  {
    _repository = repository;
    _cache = cache;
    _options = options;
  }


  public Task UpdateFeatureFlagAsync(Guid tenantId, string code, bool enabled, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
