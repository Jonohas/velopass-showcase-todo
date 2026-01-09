using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.Permissions.Entities;

namespace Shared.Permissions.Repositories;

public class PermissionRepository<TContext> : BaseRepository<Permission, Guid, TContext>, IPermissionRepository
  where TContext : DbContext
{
  public PermissionRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
  { }

  public Task<Permission?> GetNonTracking(Guid id, CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAllNonTracking))
      .AsNoTracking()
      .FirstOrDefaultAsync(cancellationToken);
  }

  public Task<List<Permission>> GetAllNonTracking(CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAllNonTracking))
      .AsNoTracking()
      .ToListAsync(cancellationToken);  }
}
