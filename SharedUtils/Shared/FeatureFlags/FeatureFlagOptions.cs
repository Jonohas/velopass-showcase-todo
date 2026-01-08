namespace Shared.FeatureFlags;

public sealed class FeatureFlagOptions
{
  /// <summary>
  /// Cache duration in seconds for effective flag sets. Defaults to 360 seconds / 5 minutes.
  /// </summary>
  public int CacheSeconds { get; set; } = 1;
}

