using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Permissions.Entities;

namespace Shared.Permissions.EntityTypeConfigurations;

public class GroupMembershipConfiguration : IEntityTypeConfiguration<GroupMembership>
{
  public void Configure(EntityTypeBuilder<GroupMembership> builder)
  {
    builder.ToTable($"{Constants.TablePrefix}_subject_group_membership");

    builder.HasKey(a => a.Id);
    builder.Property(a => a.Id).ValueGeneratedNever();

    builder.Property(a => a.SubjectId)
      .IsRequired();

    builder.Property(a => a.GroupId)
      .IsRequired();

    // Relationship to group
    builder.HasOne(m => m.Group)
      .WithMany(g => g.Memberships)
      .HasForeignKey(m => m.GroupId)
      .OnDelete(DeleteBehavior.Cascade);

    // Prevent duplicate memberships for same subject and group
    builder.HasIndex(a => new { a.SubjectId, a.GroupId })
      .IsUnique();

    builder.HasIndex(a => a.GroupId);
    builder.HasIndex(a => a.SubjectId);
  }
}
