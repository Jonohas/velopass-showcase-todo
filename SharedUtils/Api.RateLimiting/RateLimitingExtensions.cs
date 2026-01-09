using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Api.RateLimiting;

// TODO@JOREN: figure out what are good rate limit numbers
// TODO@JOREN: chain rate limiters so that we can have a global rate limiter, and user based

/// <summary>
/// Provides extension methods for configuring rate limiting in an ASP.NET Core application.
/// don't forget to add <see cref="UseRateLimiter"/> when adding rate limiting
/// </summary>
public static class RateLimitingExtensions
{
  /// <summary>
  /// Configures a global fixed window rate limiter for the application,
  /// allowing a defined number of requests within a fixed time window.
  /// </summary>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the rate limiter is added.</param>
  /// <returns>The modified <see cref="IServiceCollection"/> that includes the configured global rate limiter.</returns>
  public static IServiceCollection AddGlobalFixedWindowRateLimit(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.AddDefaultOptions();

      options.AddFixedWindowLimiter("fixed", cfg =>
      {
        cfg.PermitLimit = 100;
        cfg.Window = TimeSpan.FromMinutes(1);
      });
    });

    return services;
  }

  /// <summary>
  /// Configures a user-based rate limiter for the application, allowing different rate limits based on whether the user is identified or anonymous.
  /// </summary>
  /// <param name="services">The <see cref="IServiceCollection"/> to which the user-based rate limiter is added.</param>
  /// <returns>The modified <see cref="IServiceCollection"/> that includes the configured user-based rate limiter.</returns>
  public static IServiceCollection AddUserRateLimit(this IServiceCollection services)
  {
    services.AddRateLimiter(options =>
    {
      options.AddDefaultOptions();

      options.AddPolicy("per-user", context =>
        {
          // TODO@JOREN: userId? configurable? pass method/service like tenantService but for user to get user from context?
          var userId = context.User.FindFirstValue("userId");

          if (string.IsNullOrWhiteSpace(userId))
          {
            return RateLimitPartition.GetFixedWindowLimiter(
              "anonymous",
              _ => new FixedWindowRateLimiterOptions()
              {
                PermitLimit = 25,
                Window = TimeSpan.FromMinutes(1)
              });
          }

          // TODO@JOREN: what rate limiting method do we want? sliding window? fixed window? tokenBucket?
          // token bucket is usefull to allow short burst of many requests but don't allow high continuous load
          return RateLimitPartition.GetTokenBucketLimiter(
            userId,
            _ => new TokenBucketRateLimiterOptions()
            {
              TokenLimit = 100,
              TokensPerPeriod = 50,
              ReplenishmentPeriod = TimeSpan.FromMinutes(1)
            });
        }
      );
    });

    return services;
  }

  public static void AddDefaultOptions(this RateLimiterOptions options)
  {
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
      if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
      {
        context.HttpContext.Response.Headers.RetryAfter =
          retryAfter.TotalSeconds.ToString(CultureInfo.InvariantCulture);

        ProblemDetailsFactory problemDetailsFactory =
          context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

        ProblemDetails problemDetails =
          problemDetailsFactory.CreateProblemDetails(
            context.HttpContext,
            StatusCodes.Status429TooManyRequests,
            "Too many requests",
            detail: $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds."
          );

        await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: token);
      }
    };
  }
}
