namespace Shared.S3ObjectStorage.Implementation;

public class S3Configuration
{
  public string Endpoint { get; set; } = null!;
  public string AccessKey { get; set; } = null!;
  public string SecretKey { get; set; } = null!;
}
