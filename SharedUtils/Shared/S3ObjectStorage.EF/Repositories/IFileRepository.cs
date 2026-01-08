using Shared.Database;
using Shared.Domain;
using File = Shared.S3ObjectStorage.Domain.File.File;

namespace Shared.S3ObjectStorage.EF.Repositories;

public interface IFileRepository : IBaseRepository<File, Guid>
{
  Task<File?> GetAsync(Guid id, CancellationToken cancellationToken);
  Task<List<File>> GetAllAsync(CancellationToken cancellationToken);
  Task<List<File>> GetFilesInFolderAsync(Guid? folderId, CancellationToken cancellationToken);
}