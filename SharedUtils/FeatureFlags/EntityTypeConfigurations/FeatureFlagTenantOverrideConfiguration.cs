using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.FeatureFlags.Entities;

namespace Shared.FeatureFlags.EntityTypeConfigurations;

public class FeatureFlagTenantOverrideConfiguration : IEntityTypeConfiguration<FeatureFlagTenantOverride>
{
  public void Configure(EntityTypeBuilder<FeatureFlagTenantOverride> builder)
  {
    builder.ToTable("feature_flags_tenant_overrides");
    builder.HasKey(x => x.Id);
    builder.HasIndex(x => new { x.TenantId, x.FeatureFlagId }).IsUnique();
  }
}

