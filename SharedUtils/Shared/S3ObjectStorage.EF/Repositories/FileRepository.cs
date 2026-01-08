using Microsoft.EntityFrameworkCore;
using Shared.Database;
using File = Shared.S3ObjectStorage.Domain.File.File;

namespace Shared.S3ObjectStorage.EF.Repositories;

public class FileRepository<TContext> : BaseRepository<File, Guid, TContext>, IFileRepository
  where TContext : DbContext
{
  // public FileRepository(TContext dbContext) : base(dbContext)
  // {
  // }

  public FileRepository(IMultiTenantDbContextFactory<TContext> dbContextFactory) : base(dbContextFactory)
  {
  }

  public Task<File?> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    return Set
      .TagWith(GetType().Name + '.' + nameof(GetAsync))
      .FirstOrDefaultAsync(a => a.Id.Equals(id), cancellationToken: cancellationToken);
  }

  public Task<List<File>> GetAllAsync(CancellationToken cancellationToken)
  {
    return Set.ToListAsync(cancellationToken);
  }

  public Task<List<File>> GetFilesInFolderAsync(Guid? folderId, CancellationToken cancellationToken)
  {
    return Set.Where(file => file.ParentId == folderId)
      .ToListAsync(cancellationToken);
  }
}