using Microsoft.AspNetCore.Authorization;
using Shared.FeatureFlags.Services;

namespace Shared.FeatureFlags.Authorization;

public class FeatureFlagAuthorizationHandler(IFeatureFlagChecker checker) : AuthorizationHandler<FeatureFlagRequirement>
{
  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FeatureFlagRequirement requirement)
  {
    if (await checker.IsEnabledAsync(requirement.Code))
      context.Succeed(requirement);
  }
}

