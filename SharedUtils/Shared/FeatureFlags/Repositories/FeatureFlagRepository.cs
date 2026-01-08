using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.FeatureFlags.Entities;

namespace Shared.FeatureFlags.Repositories;

public class FeatureFlagRepository<TContext> : BaseRepository<FeatureFlag, Guid, TContext>, IFeatureFlagRepository
  where TContext : DbContext, IFeatureFlagDbContext
{
  public FeatureFlagRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory) { }
  public FeatureFlagRepository(TContext dbContext) : base(dbContext) { }

  public async Task<Dictionary<string, bool>> GetEffectiveFlagsAsync(Guid tenantId, CancellationToken cancellationToken)
  {
    var query =
      from flag in Set.AsNoTracking()
      join ov in Context.FeatureFlagTenantOverrides.AsNoTracking()
          .Where(o => o.TenantId == tenantId)
        on flag.Id equals ov.FeatureFlagId into flagOverrides
      from ov in flagOverrides.DefaultIfEmpty()
      select new
      {
        flag.Code,
        IsEnabled = ov != null ? ov.Enabled : flag.EnabledByDefault
      };

    var result = await query.ToDictionaryAsync(x => x.Code, x => x.IsEnabled, StringComparer.OrdinalIgnoreCase, cancellationToken);
    return result;
  }


  public async Task<bool?> GetEffectiveFlagAsync(Guid tenantId, string code, CancellationToken cancellationToken)
  {
    var flag = await Set
      .AsNoTracking()
      .FirstOrDefaultAsync(f => f.Code == code, cancellationToken);

    if (flag == null)
      return null; // unknown flag

    var overrideValue = await Context.FeatureFlagTenantOverrides
      .Where(o => o.TenantId == tenantId && o.FeatureFlagId == flag.Id)
      .Select(o => (bool?)o.Enabled)
      .FirstOrDefaultAsync(cancellationToken);

    return overrideValue ?? flag.EnabledByDefault;
  }

}

