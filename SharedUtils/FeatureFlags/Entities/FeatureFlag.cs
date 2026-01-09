using Shared.Domain;

namespace Shared.FeatureFlags.Entities;

public class FeatureFlag : Entity<Guid>
{
  public string Code { get; private set; } = null!; // unique, uppercase with dots (e.g., BILLING.INVOICES)
  public string? Description { get; private set; }
  public bool EnabledByDefault { get; private set; }

  private FeatureFlag() {}

  public FeatureFlag(string code, string? description, bool enabledByDefault)
  {
    Id = Guid.CreateVersion7();
    Code = code.Trim();
    Description = description;
    EnabledByDefault = enabledByDefault;
  }
}

