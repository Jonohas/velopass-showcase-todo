using Microsoft.EntityFrameworkCore;
using Shared.FeatureFlags.EntityTypeConfigurations;

namespace Shared.FeatureFlags;

public static class ModelBuilderExtensions
{
  public static ModelBuilder ApplyFeatureFlagConfigurations(this ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(FeatureFlagConfiguration).Assembly);
    return modelBuilder;
  }
}

