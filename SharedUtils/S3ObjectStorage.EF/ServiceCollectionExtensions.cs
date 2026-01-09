using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.S3ObjectStorage.EF.Repositories;

namespace Shared.S3ObjectStorage.EF;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddObjectStorageRepositories<TContext>(this IServiceCollection services)
    where TContext : DbContext
  {
    services.AddScoped<IFileRepository, FileRepository<TContext>>();
    services.AddScoped<IFolderRepository, FolderRepository<TContext>>();
    return services;
  }
}