using Shared.FeatureFlags.Interfaces;

namespace Shared.FeatureFlags.Services;

public class FeatureFlagChecker : IFeatureFlagChecker
{
  private readonly IFeatureFlagAccessService _accessService;

  public FeatureFlagChecker(IFeatureFlagAccessService accessService)
  {
    _accessService = accessService;
  }

  public Task<bool> IsEnabledAsync(string code, CancellationToken cancellationToken = default)
  {
    return _accessService.HasAccessAsync(code, cancellationToken);;
  }
}

