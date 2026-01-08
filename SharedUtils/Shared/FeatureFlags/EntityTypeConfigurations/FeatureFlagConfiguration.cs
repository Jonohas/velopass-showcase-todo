using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.FeatureFlags.Entities;

namespace Shared.FeatureFlags.EntityTypeConfigurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
  public void Configure(EntityTypeBuilder<FeatureFlag> builder)
  {
    builder.ToTable("feature_flags");
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Code).IsRequired().HasMaxLength(200);
    builder.HasIndex(x => x.Code).IsUnique();
    builder.Property(x => x.EnabledByDefault).IsRequired();
  }
}

