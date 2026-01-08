using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Permissions.Entities;

namespace Shared.Permissions.EntityTypeConfigurations;

public class GroupPermissionConfiguration : IEntityTypeConfiguration<GroupPermission>
{
  public void Configure(EntityTypeBuilder<GroupPermission> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_group_permission");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    builder.Property(a => a.GroupId)
      .IsRequired();

    builder.Property(a => a.PermissionId)
      .IsRequired();

    // Relationships
    builder.HasOne(gp => gp.Group)
      .WithMany(g => g.GroupPermissions)
      .HasForeignKey(gp => gp.GroupId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasOne(gp => gp.Permission)
      .WithMany(p => p.GroupPermissions)
      .HasForeignKey(gp => gp.PermissionId)
      .OnDelete(DeleteBehavior.Cascade);

    // Prevent duplicate assignments
    builder.HasIndex(a => new { a.GroupId, a.PermissionId })
      .IsUnique();

    builder.HasIndex(a => a.GroupId);
    builder.HasIndex(a => a.PermissionId);
  }
}
