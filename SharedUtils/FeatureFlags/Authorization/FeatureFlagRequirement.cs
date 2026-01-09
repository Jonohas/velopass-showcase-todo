using Microsoft.AspNetCore.Authorization;

namespace Shared.FeatureFlags.Authorization;

public sealed class FeatureFlagRequirement(string code) : IAuthorizationRequirement
{
  public string Code { get; } = code;
}

