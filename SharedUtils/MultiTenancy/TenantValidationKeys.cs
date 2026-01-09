using JV.ResultUtilities.Extensions;
using JV.ResultUtilities.ValidationMessage;

namespace Shared.MultiTenancy;

public static class TenantValidationKeys
{
  public static class Tenant
  {
    public static readonly ValidationKeyDefinition ForcedFailure = ValidationKeyDefinition.Create("Tenant","ForcedFailure");
    public static readonly ValidationKeyDefinition RealmNameRequired = ValidationKeyDefinition.Create("Tenant","RealmNameRequired");
    public static readonly ValidationKeyDefinition RealmNameInvalidChar = ValidationKeyDefinition.Create("Tenant","RealmNameInvalidChar")
      .WithStringParameter("Char");
    public static readonly ValidationKeyDefinition ConnectionStringRequired = ValidationKeyDefinition.Create("Tenant","ConnectionStringRequired");
    public static readonly ValidationKeyDefinition ConnectionStringMissingToken = ValidationKeyDefinition.Create("Tenant","ConnectionStringMissingToken")
      .WithStringParameter("Token");
  }
}

