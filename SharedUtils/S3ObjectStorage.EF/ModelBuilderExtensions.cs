using Microsoft.EntityFrameworkCore;
using Shared.S3ObjectStorage.EF.EntityTypeConfigurations;

namespace Shared.S3ObjectStorage.EF;

public static class ModelBuilderExtensions
{
  /// <summary>
  /// Applies EF Core configurations for Shared.S3ObjectStorage.Domain entities (File, Folder).
  /// Call this from your DbContext.OnModelCreating.
  /// </summary>
  public static ModelBuilder ApplyObjectStorageConfigurations(this ModelBuilder modelBuilder)
  {
    // Apply all configurations in this assembly
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileConfiguration).Assembly);
    return modelBuilder;
  }
}