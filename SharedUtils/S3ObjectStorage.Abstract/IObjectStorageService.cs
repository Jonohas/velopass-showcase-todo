using JV.ResultUtilities;

namespace Shared.S3ObjectStorage.Abstract;

public interface IObjectStorageService
{
  Task<Result<UploadResult>> UploadAsync(string bucket, string objectName, Stream data, long size, string contentType,
    CancellationToken cancellationToken);

  Task<Result<string>> GetPresignedGetUrlAsync(string bucket, string objectName, int expirySeconds,
    CancellationToken cancellationToken);

  Task<Result<bool>> BucketExistsAsync(string bucket, CancellationToken cancellationToken);
}

public record UploadResult(string ObjectName, long Size);
