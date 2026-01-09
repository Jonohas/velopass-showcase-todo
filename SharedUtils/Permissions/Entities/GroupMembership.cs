using Shared.Domain;

namespace Shared.Permissions.Entities;

public class GroupMembership : Entity<Guid>
{
  public Guid SubjectId { get; private set; }
  public Guid GroupId { get; private set; }

  public GroupMembership(Guid subjectId, Guid groupId)
  {
    Id = Guid.CreateVersion7();
    SubjectId = subjectId;
    GroupId = groupId;
  }

  // Navigation
  public PermissionGroup Group { get; set; } = default!;
}
