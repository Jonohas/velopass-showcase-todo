using Microsoft.AspNetCore.Authorization;

namespace Shared.FeatureFlags.Authorization;

/// <summary>
/// Generates policies on-the-fly for policies that start with Feature:
/// </summary>
public class FeatureFlagPolicyProvider(IAuthorizationPolicyProvider fallback) : IAuthorizationPolicyProvider
{
  public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => fallback.GetDefaultPolicyAsync();
  public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => fallback.GetFallbackPolicyAsync();

  public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
  {
    if (policyName.StartsWith(FeatureAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
    {
      var code = policyName[FeatureAttribute.PolicyPrefix.Length..];
      var policy = new AuthorizationPolicyBuilder()
        .AddRequirements(new FeatureFlagRequirement(code))
        .Build();
      return Task.FromResult<AuthorizationPolicy?>(policy);
    }
    return fallback.GetPolicyAsync(policyName);
  }
}

