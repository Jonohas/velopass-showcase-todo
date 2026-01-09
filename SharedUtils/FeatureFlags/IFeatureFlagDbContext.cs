using Microsoft.EntityFrameworkCore;
using Shared.FeatureFlags.Entities;

namespace Shared.FeatureFlags;

public interface IFeatureFlagDbContext
{
  DbSet<FeatureFlag> FeatureFlags { get; }
  DbSet<FeatureFlagTenantOverride> FeatureFlagTenantOverrides { get; }
}

