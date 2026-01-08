using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Permissions.Entities;

namespace Shared.Permissions.Repositories;

public class PermissionGroupRepository<TContext> : BaseRepository<PermissionGroup, Guid, TContext>, IPermissionGroupRepository
  where TContext : DbContext
{
  public PermissionGroupRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
  { }

  public Task<PermissionGroup?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAsync))
      .FirstOrDefaultAsync(cancellationToken);
  }

  public Task<List<PermissionGroup>> GetAllIncludingPermissionsAsync(CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAllIncludingPermissionsAsync))
      .Include(x => x.GroupPermissions)
      .ThenInclude(gp => gp.Permission)
      .ToListAsync(cancellationToken);
  }

  public Task<List<PermissionGroup>> GetAllIncludingPermissionsAndMembershipsAsync(CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAllIncludingPermissionsAndMembershipsAsync))
      .Include(x => x.GroupPermissions)
      .ThenInclude(gp => gp.Permission)
      .Include(x => x.Memberships)
      .ToListAsync(cancellationToken);
  }
}
