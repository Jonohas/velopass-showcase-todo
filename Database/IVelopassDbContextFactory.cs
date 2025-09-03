using Microsoft.EntityFrameworkCore;

namespace Database;

public interface IVelopassDbContextFactory<TDbContext> : IDisposable
    where TDbContext : DbContext
{
    /// <summary>
    /// Create or reuse an existing DbContext
    /// </summary>
    TDbContext Create();

    /// <summary>
    /// Always creates a new DbContext
    /// </summary>
    TDbContext CreateNew();

    /// <summary>
    /// Always creates a new DbContext for backgroundservice
    /// </summary>
    TDbContext CreateNewInMemory();
}
