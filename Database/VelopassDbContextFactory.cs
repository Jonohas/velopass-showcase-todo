using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;

namespace Database;

public class TodoDbContextFactory : ICustomDbContextFactory<TodoDbContext>
{
  private readonly string _connectionString;
  private readonly bool _enableSensitiveDataLogging;
  private TodoDbContext? _dbContext;

  public TodoDbContextFactory(string connectionString, IConfiguration configuration)
  {
    _connectionString = connectionString;
    var enableQueryLoggingString = configuration["Debugging:EnableQueryLogging"];
    if (!bool.TryParse(enableQueryLoggingString, out _enableSensitiveDataLogging))
    {
      throw new ArgumentException("EnableQueryLogging configuration is not valid");
    }

    _dbContext = CreateDbContext();
  }

  public TodoDbContext CreateDbContext()
  {
    if (_dbContext is null)
      _dbContext = CreateNew();

    return _dbContext;
  }

  public TodoDbContext CreateNew()
  {
    var dbOptions = new DbContextOptionsBuilder<TodoDbContext>()
      .UseNpgsql(_connectionString,
        options =>
        {
          options.MigrationsAssembly(typeof(TodoDbContextFactory).Assembly.FullName);
          options.MigrationsHistoryTable(TodoDbContext.MigrationsSchema, TodoDbContext.MigrationsSchema);
        })
      .UseSnakeCaseNamingConvention();

    if (_enableSensitiveDataLogging)
    {
      dbOptions
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, (eventId, logLevel) => logLevel >= LogLevel.Information
                                                         || eventId == RelationalEventId.DataReaderDisposing);
    }

    _dbContext = new TodoDbContext(dbOptions.Options);

    return _dbContext;
  }

  public void Dispose()
  {
    _dbContext?.Dispose();
  }

  public async ValueTask DisposeAsync()
  {
    if (_dbContext != null)
      await _dbContext.DisposeAsync();
  }
}