using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Permissions.Entities;

namespace Shared.Permissions.EntityTypeConfigurations;

public class PermissionGroupConfiguration : IEntityTypeConfiguration<PermissionGroup>
{
  public void Configure(EntityTypeBuilder<PermissionGroup> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_group");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    builder.Property(a => a.Name)
      .IsRequired()
      .HasMaxLength(255);

    builder.Property(a => a.Description)
      .HasMaxLength(510);

    builder.HasIndex(a => a.Name);

    // Relationships
    builder.HasMany(g => g.GroupPermissions)
      .WithOne(gp => gp.Group)
      .HasForeignKey(gp => gp.GroupId)
      .OnDelete(DeleteBehavior.Cascade);

    builder.HasMany(g => g.Memberships)
      .WithOne(m => m.Group)
      .HasForeignKey(m => m.GroupId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
