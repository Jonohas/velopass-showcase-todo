using Shared.FeatureFlags.Entities;

namespace Shared.FeatureFlags.Interfaces;

public interface IFeatureFlagAccessService
{
  Task<bool> HasAccessAsync(string featureCode, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<FeatureFlag>> GetAllAsync(CancellationToken cancellationToken = default);
  Task InvalidateAsync(string? featureCode = null, CancellationToken cancellationToken = default);
}