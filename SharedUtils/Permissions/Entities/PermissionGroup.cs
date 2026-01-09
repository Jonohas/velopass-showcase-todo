using Shared.Domain;

namespace Shared.Permissions.Entities;

public class PermissionGroup : Entity<Guid>
{
  public string Name { get; private set; } = default!;
  public string? Description { get; private set; }

  public PermissionGroup(string name, string? description = null)
  {
    Id = Guid.CreateVersion7();
    Name = name;
    Description = description;
  }

  // Navigation
  public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
  public ICollection<GroupMembership> Memberships { get; set; } = new List<GroupMembership>();
}
