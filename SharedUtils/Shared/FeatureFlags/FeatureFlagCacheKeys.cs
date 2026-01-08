namespace Shared.FeatureFlags;

public static class FeatureFlagCacheKeys
{
  public const string Base = "FeatureFlags:";
  public static string EffectiveByTenant(Guid tenantId) => Base + $"Effective:{tenantId}";
  public static string Single(Guid tenantId, string code) => Base + $"Single:{tenantId}:{code}";
}

