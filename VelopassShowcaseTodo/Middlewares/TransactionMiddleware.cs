using Database;
using Microsoft.EntityFrameworkCore;

namespace VelopassShowcaseTodo.Middlewares;

public static class TransactionMiddlewareExtensions
{
  /// <summary>
  /// Registers <see cref="TransactionMiddleware"/> in the request pipeline.
  /// Must be placed AFTER <see cref="TodoMiddleware"/>.
  /// </summary>
  public static IApplicationBuilder UseTransactionMiddleware(this IApplicationBuilder app)
  {
    return app.UseMiddleware<TransactionMiddleware>();
  }
}

/// <summary>
/// Starts a database transaction for every Todo request and commits
/// (or rolls-back) it once the downstream pipeline has finished.
/// Requests that target the /admin* routes are ignored.
///
/// Prerequisite:
/// • The preceding TodoMiddleware must have stored the Todo
///   connection string in HttpContext.Items["TodoConnectionString"].
/// </summary>
public sealed class TransactionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<TransactionMiddleware> _logger;

  public TransactionMiddleware(RequestDelegate next, ILogger<TransactionMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context,
    TodoDbContext todoDbContext)
  {
    await AddTransactionAsync(context, todoDbContext);
  }

  private async Task AddTransactionAsync(HttpContext context, DbContext dbContext)
  {
    // 4. Start the transaction
    await using var transaction = await dbContext.Database.BeginTransactionAsync(context.RequestAborted);

    try
    {
      // 5. Execute the rest of the pipeline
      await _next(context);

      // 6. Commit only when the pipeline completed successfully
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
      _logger.LogError("Error while processing Todo request – transaction rolled back.");
      throw; // Let the global exception handler convert this to the proper response
    }
  }
}