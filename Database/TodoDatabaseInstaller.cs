using Domain.Repositories;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Database;

public static class TodoDatabaseInstaller
{
    public static IServiceCollection AddTodoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Fetch the connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (connectionString == null)
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // Register the TodoDbContext
        services.AddDbContext<TodoDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString, builder =>
                {
                    // Configure database-specific behavior
                    builder.SetPostgresVersion(17, 0);
                    builder.ConfigureDataSource(dataSourceBuilder => dataSourceBuilder.EnableDynamicJson());
                    builder.MigrationsAssembly(typeof(TodoDbContext).Assembly.FullName);
                    builder.MigrationsHistoryTable(TodoDbContext.MigrationsSchema, TodoDbContext.MigrationsSchema);
                })
                .UseSnakeCaseNamingConvention();

            // Enable query logging if specified in the configuration
            var enableQueryLoggingString = configuration["Debugging:EnableQueryLogging"];
            if (bool.TryParse(enableQueryLoggingString, out bool isQueryLoggingEnabled) && isQueryLoggingEnabled)
            {
                options
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, (eventId, logLevel) => logLevel >= LogLevel.Information
                                                                     || eventId == RelationalEventId.DataReaderDisposing);
            }
        });

        services.AddTodoRepositories();
        services.AddTodoInfrastructure();

        return services;
    }
    
    public static IServiceCollection AddTodoRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
    
    public static IServiceCollection AddTodoInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}