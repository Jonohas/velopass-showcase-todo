using Microsoft.AspNetCore.Authorization;

namespace Shared.FeatureFlags.Authorization;

/// <summary>
/// Attribute to protect endpoints by feature flag. Usage: [Feature("MY.FEATURE")]
/// </summary>
public sealed class FeatureAttribute : AuthorizeAttribute
{
  public const string PolicyPrefix = "Feature:";
  public string Code { get; }

  public FeatureAttribute(string code)
  {
    Code = code;
    Policy = PolicyPrefix + code;
  }
}

