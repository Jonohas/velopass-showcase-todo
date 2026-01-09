using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Shared.Api.Tests.WebApplicationFactories;

/// <summary>
/// Base factory for creating test servers for E2E API testing.
/// Provides common configuration for database, authentication, and service mocking.
/// </summary>
/// <typeparam name="TProgram">The program/startup class of the API being tested</typeparam>
/// <typeparam name="TDbContext">The DbContext type used by the API</typeparam>
public class ApiWebApplicationFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>
  where TProgram : class
  where TDbContext : DbContext
{
  private static readonly ConcurrentDictionary<string, string?> Config = new();
  private static readonly ConfigurationReloadToken ReloadToken = new();
  protected bool UseInMemoryDatabase = true;
  

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");
    ConfigureTestWebHostBuilder(builder);

    builder.ConfigureAppConfiguration((context, config) =>
    {
      Config["Environment"] = "Testing";
      Config["Database:RunMigrationOnStartup"] = "false";
      Config["Serilog:MinimumLevel:Default"] = "Warning";
      config.Add(new DynamicDictionaryConfigurationSource(Config, ReloadToken));
      
      ConfigureTestConfiguration(config);
    });
   
    // TODO@JOREN: do we need this?
    // builder.ConfigureLogging(logging =>
    // {
    //   logging.ClearProviders();
    //   logging.AddConsole();
    // });

    builder.ConfigureTestServices(services =>
    {
      SetupDatabase(services);

      ConfigureTestServices(services);
    });
  }

  private void SetupDatabase(IServiceCollection services)
  {
    // Remove the existing DbContext registration
    services.RemoveAll(typeof(DbContextOptions<TDbContext>));
    services.RemoveAll(typeof(TDbContext));

    if (UseInMemoryDatabase)
    {
      // Use in-memory database for faster tests
      services.AddDbContext<TDbContext>(options =>
      {
        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
        options.EnableSensitiveDataLogging();
      });
    }
    else
    {
      services.AddSingleton<PostgreSqlBuilder>(_ => new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .WithUsername("Test")
        .WithPassword("Test")
        .WithDatabase("Test"));
    }
  }

  // overridable methods for derived classes to customize behavior
  protected virtual void ConfigureTestWebHostBuilder(IWebHostBuilder builder) { }
  protected virtual void ConfigureTestConfiguration(IConfigurationBuilder builder) { }
  protected virtual void ConfigureTestServices(IServiceCollection services) { }
}
