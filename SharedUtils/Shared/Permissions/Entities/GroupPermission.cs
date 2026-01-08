using Shared.Domain;

namespace Shared.Permissions.Entities;

public class GroupPermission : Entity<Guid>
{
  public Guid GroupId { get; private set; }
  public Guid PermissionId { get; private set; }

  public GroupPermission(Guid groupId, Guid permissionId)
  {
    Id = Guid.CreateVersion7();
    GroupId = groupId;
    PermissionId = permissionId;
  }

  // Navigation
  public PermissionGroup Group { get; set; } = default!;
  public Permission Permission { get; set; } = default!;
}
