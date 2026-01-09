using Shared.Database;
using Shared.Domain;
using Shared.Permissions.Entities;

namespace Shared.Permissions.Repositories;

public interface IGroupMembershipRepository : IBaseRepository<GroupMembership, Guid>
{
  Task<List<string>> GetPermissionCodesAsync(Guid subjectId, CancellationToken cancellationToken);
  Task<List<GroupMembership>> GetAllNonTracking(CancellationToken cancellationToken);
}
