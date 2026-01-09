using Database;
using Microsoft.EntityFrameworkCore;

namespace VelopassShowcaseTodo.Middlewares;

public static class TransactionMiddlewareExtensions
{
  public static IApplicationBuilder UseTransactionMiddleware(this IApplicationBuilder app)
  {
    return app.UseMiddleware<TransactionMiddleware>();
  }
}

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
    await using var transaction = await dbContext.Database.BeginTransactionAsync(context.RequestAborted);

    try
    {
      await _next(context);
      
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
      throw;
    }
  }
}