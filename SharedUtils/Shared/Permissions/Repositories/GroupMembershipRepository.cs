using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Permissions.Entities;

namespace Shared.Permissions.Repositories;

public class GroupMembershipRepository<TContext> : BaseRepository<GroupMembership, Guid, TContext>, IGroupMembershipRepository
  where TContext : DbContext
{
  public GroupMembershipRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
  {
  }

  public Task<List<string>> GetPermissionCodesAsync(Guid subjectId, CancellationToken cancellationToken)
  {
    return Set
      .Where(m => m.SubjectId == subjectId)
      .Join(Context.Set<GroupPermission>(), m => m.GroupId, gp => gp.GroupId, (m, gp) => gp)
      .Join(Context.Set<Permission>(), gp => gp.PermissionId, p => p.Id, (gp, p) => p.Code)
      .Distinct()
      .ToListAsync(cancellationToken);
  }

  public Task<List<GroupMembership>> GetAllNonTracking(CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAllNonTracking))
      .AsNoTracking()
      .ToListAsync(cancellationToken);
  }
}
