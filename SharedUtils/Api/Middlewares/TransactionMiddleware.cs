using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace Shared.Api.Middlewares;

public static class TransactionMiddlewareExtensions
{
  /// <summary>
  /// Registers <see cref="TransactionMiddleware"/> in the request pipeline.
  /// Must be placed AFTER <see cref="TenantMiddleware"/>.
  /// </summary>
  public static IApplicationBuilder UseTransactionMiddleware<T>(this IApplicationBuilder app)
    where T : DbContext
  {
    return app.UseMiddleware<TransactionMiddleware<T>>();
  }
}

public class TransactionMiddleware<T>
  where T : DbContext
{
  private readonly RequestDelegate _next;
  private readonly ILogger<TransactionMiddleware<T>> _logger;
  private readonly List<string> _methodsToSkip = ["GET", "OPTIONS", "HEAD"];

  public TransactionMiddleware(RequestDelegate next, ILogger<TransactionMiddleware<T>> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context, T dbContext)
  {
    // let openapi calls pass
    if (_methodsToSkip.Contains(context.Request.Method) || context.Request.Path.IsOpenApiRequest())
    {
      await _next(context);
      return;
    }

    // Start the transaction
    await using var transaction = await dbContext.Database.BeginTransactionAsync(context.RequestAborted);

    try
    {
      // Execute the rest of the pipeline
      await _next(context);

      // Commit only when the pipeline completed successfully
      if (context.Response.StatusCode < 400)
      {
        await dbContext.SaveChangesAsync(context.RequestAborted);
        await transaction.CommitAsync(context.RequestAborted);
      }
      else
      {
        await transaction.RollbackAsync(context.RequestAborted);
      }
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync(context.RequestAborted);
      _logger.LogError("Error while processing tenant request – transaction rolled back. exception: {ex}", ex);
      throw; // Let the global exception handler convert this to the proper response
    }
  }
}
