using Shared.Database;
using Shared.Domain;
using Shared.FeatureFlags.Entities;

namespace Shared.FeatureFlags.Repositories;

public interface IFeatureFlagRepository : IBaseRepository<FeatureFlag, Guid>
{
  Task<Dictionary<string, bool>> GetEffectiveFlagsAsync(Guid tenantId, CancellationToken cancellationToken);
  Task<bool?> GetEffectiveFlagAsync(Guid tenantId, string code, CancellationToken cancellationToken);
}

