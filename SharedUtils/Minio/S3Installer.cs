using Microsoft.Extensions.DependencyInjection;
using Minio;
using Shared.S3ObjectStorage.Abstract;

namespace Shared.S3ObjectStorage.Implementation;

public static class S3Installer
{
  public static IServiceCollection AddS3ObjectStorage(this IServiceCollection services, S3Configuration configuration)
  {
    services.AddMinio(configureClient => configureClient
      .WithEndpoint(configuration.Endpoint)
      .WithCredentials(configuration.AccessKey, configuration.SecretKey)
      .Build());

    services.AddScoped<IObjectStorageService>(sp =>
    {
      var factory = sp.GetRequiredService<IMinioClientFactory>();
      var client = factory.CreateClient();
      return new S3ObjectStorageServiceService(client);
    });

    return services;
  }
}
