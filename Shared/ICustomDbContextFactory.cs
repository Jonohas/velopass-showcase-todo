using Microsoft.EntityFrameworkCore;

namespace Shared;

public interface ICustomDbContextFactory<TDbContext> : IAsyncDisposable, IDisposable, IDbContextFactory<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Always creates a new DbContext
    /// </summary>
    TDbContext CreateNew();
}
