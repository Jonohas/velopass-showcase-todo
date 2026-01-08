using Microsoft.EntityFrameworkCore;

namespace Shared.Database;

public class ReadOnlyDbContext : DbContext
{
  public ReadOnlyDbContext(DbContextOptions options)
    : base(options)
  {
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
  }

  public override int SaveChanges() =>
    throw new InvalidOperationException("This context is read-only.");

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
    throw new InvalidOperationException("This context is read-only.");
}
