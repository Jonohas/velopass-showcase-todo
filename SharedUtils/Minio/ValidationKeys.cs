using JV.ResultUtilities.Extensions;
using JV.ResultUtilities.ValidationMessage;

namespace Shared.S3ObjectStorage.Implementation;

public static class ValidationKeys
{
  public static class S3
  {
    public static readonly ValidationKeyDefinition UploadFailed = ValidationKeyDefinition.Create("S3.UploadFailed")
      .WithStringParameter("FileName");

    public static readonly ValidationKeyDefinition GetSignedUrlFailed = ValidationKeyDefinition
      .Create("S3.GetSignedUrlFailed")
      .WithStringParameter("FileId");

    public static readonly ValidationKeyDefinition ConnectionFailed =
      ValidationKeyDefinition.Create("S3.ConnectionFailed");

    public static readonly ValidationKeyDefinition BucketNotFound = ValidationKeyDefinition
      .Create("S3.BucketNotFound")
      .WithStringParameter("BucketName");
  }
}
