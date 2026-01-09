using Microsoft.EntityFrameworkCore;
using Shared.Database;
using Shared.S3ObjectStorage.Domain.Folder;

namespace Shared.S3ObjectStorage.EF.Repositories;

public class FolderRepository<TContext> : BaseRepository<Folder, Guid, TContext>, IFolderRepository
  where TContext : DbContext
{
  // public FolderRepository(TContext dbContext) : base(dbContext)
  // {
  // }

  public FolderRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
  {
  }
  
  public Task<List<Folder>> GetFoldersInFolderAsync(Guid? folderId, CancellationToken cancellationToken)
  {
    return Set.Where(folder => folder.ParentId == folderId)
      .ToListAsync(cancellationToken);
  }

  public Task<Folder?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAsync))
      .FirstOrDefaultAsync(a => a.Id.Equals(id), cancellationToken: cancellationToken);
  }
}