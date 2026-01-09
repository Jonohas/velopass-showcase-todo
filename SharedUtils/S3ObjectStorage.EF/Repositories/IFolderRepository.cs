using Shared.Database;
using Shared.Domain;
using Shared.S3ObjectStorage.Domain.Folder;

namespace Shared.S3ObjectStorage.EF.Repositories;

public interface IFolderRepository : IBaseRepository<Folder, Guid>
{
  Task<List<Folder>> GetFoldersInFolderAsync(Guid? folderId, CancellationToken cancellationToken);
  Task<Folder?> GetAsync(Guid id, CancellationToken cancellationToken);
}