using Shared.Domain;

namespace Shared.Permissions.Entities;

public sealed class Permission : Entity<Guid>
{
  public string Code { get; private set; } // unique, stable identifier used in code
  public string Name { get; private set; } // human readable
  public string? Description { get; private set; } // TODO@JOREN: translate? postgres json translations

  public Permission(string code, string name, string? description = null)
  {
    Id = Guid.CreateVersion7();
    Code = code;
    Name = name;
    Description = description;
  }

  // Navigation
  public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}
