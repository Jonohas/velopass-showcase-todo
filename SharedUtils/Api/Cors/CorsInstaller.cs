using Microsoft.Extensions.DependencyInjection;

namespace Shared.Api.Cors;

public static class CorsInstaller
{
  public const string CorsPolicy = "CorsPolicy";

  public static IServiceCollection AddCors(this IServiceCollection services, CorsConfiguration config)
  {
    return services.AddCors(options =>
    {
      options.AddPolicy(CorsPolicy, builder =>
      {
        builder.WithOrigins(config.AllowedOrigins)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
      });
    });
  }
}
