namespace Shared.Database;

public class BaseDatabaseConfiguration
{
  public bool EnableQueryLogging { get; set; }
  public bool RunMigrationOnStartup { get; set; }
}

public class SingleTenantDatabaseConfiguration : BaseDatabaseConfiguration
{
  public required string ConnectionString { get; set; }
}
