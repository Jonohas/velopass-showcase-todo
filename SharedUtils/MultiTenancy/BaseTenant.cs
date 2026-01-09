// ServiceCruiser is our proprietary software and all source code, databases, functionality, software, website designs, audio, video, text, photographs, graphics (collectively referred to as the ‘Content’) and all intellectual property rights, including all copyright, all trademarks, all logos and all know-how vested therein or related thereto  (collectively referred to as the ‘IPR’) are owned, licenced or controlled by ESAS 3Services NV (or any of its affiliates or subsidiaries), excluded is Content or IPR provided and owned by third parties. All Content and IPR are protected by copyright and trademark laws and various other intellectual property rights legislation and/or other European Union and/or Belgian legislation, including unfair commercial practices legislation.

using System.Text.Json.Serialization;
using Shared.Domain;

namespace Shared.MultiTenancy;

public abstract class BaseTenant : Entity<Guid>, ITenant
{
  public string Name { get; protected set; } = null!;
  public string ConnectionString { get; protected set; } = null!;
  public string RealmName { get; protected set; } = null!;

  protected BaseTenant(string name, string connectionString, string realmName)
  {
    Id = Guid.CreateVersion7();
    Name = name;
    ConnectionString = connectionString;
    RealmName = realmName;
  }

  [JsonConstructor]
  [Obsolete("only for json constructor, don't call this manually")]
  protected BaseTenant(Guid id, string name, string connectionString, string realmName)
  {
    Id = id;
    Name = name;
    ConnectionString = connectionString;
    RealmName = realmName;
  }
}
