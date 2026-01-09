using Shared.Domain;

namespace Shared.FeatureFlags.Entities;

public class FeatureFlagTenantOverride : Entity<Guid>
{
  public Guid TenantId { get; private set; }
  public Guid FeatureFlagId { get; private set; }
  public bool Enabled { get; private set; }

  private FeatureFlagTenantOverride() {}

  public FeatureFlagTenantOverride(Guid tenantId, Guid featureFlagId, bool enabled)
  {
    Id = Guid.CreateVersion7();
    TenantId = tenantId;
    FeatureFlagId = featureFlagId;
    Enabled = enabled;
  }

  public void Set(bool enabled) => Enabled = enabled;
}

