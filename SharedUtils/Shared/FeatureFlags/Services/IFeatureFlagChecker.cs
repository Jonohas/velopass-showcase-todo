namespace Shared.FeatureFlags.Services;

public interface IFeatureFlagChecker
{
  Task<bool> IsEnabledAsync(string code, CancellationToken cancellationToken = default);
}

