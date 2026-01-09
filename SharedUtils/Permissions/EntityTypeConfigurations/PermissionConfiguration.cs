using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Permissions.Entities;

namespace Shared.Permissions.EntityTypeConfigurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
  public void Configure(EntityTypeBuilder<Permission> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_permission");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    builder.Property(a => a.Code)
      .IsRequired()
      .HasMaxLength(255);

    builder.Property(a => a.Name)
      .IsRequired()
      .HasMaxLength(255);

    builder.Property(a => a.Description)
      .HasMaxLength(510);

    builder.HasIndex(a => a.Code).IsUnique();
    builder.HasIndex(a => a.Name);

    // Explicit relationship: Permission 1 - * GroupPermission
    builder.HasMany(p => p.GroupPermissions)
      .WithOne(gp => gp.Permission)
      .HasForeignKey(gp => gp.PermissionId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
