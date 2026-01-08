using Shared.Database;
using Shared.Permissions.Entities;

namespace Shared.Permissions.Repositories;

public interface IPermissionGroupRepository : IBaseRepository<PermissionGroup, Guid>
{
  Task<PermissionGroup?> GetAsync(Guid id, CancellationToken cancellationToken);
  Task<List<PermissionGroup>> GetAllIncludingPermissionsAsync(CancellationToken cancellationToken);
  Task<List<PermissionGroup>> GetAllIncludingPermissionsAndMembershipsAsync(CancellationToken cancellationToken);
}
