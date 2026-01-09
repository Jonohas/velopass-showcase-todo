using JV.ResultUtilities;
using JV.ResultUtilities.ValidationMessage;
using Minio;
using Minio.DataModel.Args;
using Shared.S3ObjectStorage.Abstract;

namespace Shared.S3ObjectStorage.Implementation;

internal class S3ObjectStorageServiceService : IObjectStorageService
{
  private readonly IMinioClient _client;

  public S3ObjectStorageServiceService(IMinioClient client)
  {
    _client = client;
  }

  public async Task<Result<UploadResult>> UploadAsync(string bucket, string objectName, Stream data, long size,
    string contentType, CancellationToken cancellationToken)
  {
    try
    {
      var putObjectArgs = new PutObjectArgs()
        .WithBucket(bucket)
        .WithObject(objectName)
        .WithStreamData(data)
        .WithObjectSize(size)
        .WithContentType(contentType);

      var response = await _client.PutObjectAsync(putObjectArgs, cancellationToken).ConfigureAwait(false);
      return Result.Ok(new UploadResult(response.ObjectName, response.Size));
    }
    catch (Exception)
    {
      return Result.Error(ValidationMessage.Create(ValidationKeys.S3.UploadFailed, objectName));
    }
  }

  public async Task<Result<string>> GetPresignedGetUrlAsync(string bucket, string objectName, int expirySeconds,
    CancellationToken cancellationToken)
  {
    try
    {
      var args = new PresignedGetObjectArgs()
        .WithBucket(bucket)
        .WithObject(objectName)
        .WithExpiry(expirySeconds);
      var url = await _client.PresignedGetObjectAsync(args).ConfigureAwait(false);
      if (string.IsNullOrWhiteSpace(url))
        return Result.Error(ValidationMessage.Create(ValidationKeys.S3.GetSignedUrlFailed, objectName));
      return Result.Ok(url);
    }
    catch (Exception)
    {
      return Result.Error(ValidationMessage.Create(ValidationKeys.S3.GetSignedUrlFailed, objectName));
    }
  }

  public async Task<Result<bool>> BucketExistsAsync(string bucket, CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(bucket))
      return Result.Error(ValidationKeys.S3.BucketNotFound);

    try
    {
      var beArgs = new BucketExistsArgs().WithBucket(bucket);
      var exists = await _client.BucketExistsAsync(beArgs, cancellationToken).ConfigureAwait(false);
      return Result.Ok(exists);
    }
    catch (NullReferenceException ex)
    {
      // Minio SDK internal null; surface clearer error.
      return Result.Error(ValidationMessage.Create(ValidationKeys.S3.ConnectionFailed, $"NullRef: {ex.Message}"));
    }
    catch (Exception ex)
    {
      // Optionally log ex
      return Result.Error(ValidationKeys.S3.ConnectionFailed);
    }
  }
}
