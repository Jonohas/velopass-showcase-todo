using Shared.Database;
using Shared.Domain;
using Shared.Permissions.Entities;

namespace Shared.Permissions.Repositories;

public interface IPermissionRepository : IBaseRepository<Permission, Guid>
{
  Task<Permission?> GetNonTracking(Guid id, CancellationToken cancellationToken);
  Task<List<Permission>> GetAllNonTracking(CancellationToken cancellationToken);
}
