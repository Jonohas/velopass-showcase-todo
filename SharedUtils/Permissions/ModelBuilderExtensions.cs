using Microsoft.EntityFrameworkCore;
using Shared.Permissions.EntityTypeConfigurations;

namespace Shared.Permissions;

public static class ModelBuilderExtensions
{
  /// <summary>
  /// Applies EF Core configurations for Shared.Permissions entities.
  /// Call this from your DbContext.OnModelCreating.
  /// </summary>
  public static ModelBuilder ApplyPermissionsConfigurations(this ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(PermissionConfiguration).Assembly);
    return modelBuilder;
  }
}
