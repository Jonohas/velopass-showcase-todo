using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Permissions.Repositories;

namespace Shared.Permissions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddPermissionRepositories<TContext>(this IServiceCollection services)
    where TContext : DbContext
  {
    services.AddScoped<IPermissionRepository, PermissionRepository<TContext>>();
    services.AddScoped<IPermissionGroupRepository, PermissionGroupRepository<TContext>>();
    services.AddScoped<IGroupMembershipRepository, GroupMembershipRepository<TContext>>();
    return services;
  }
}
