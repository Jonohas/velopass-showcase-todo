namespace Shared.Keycloak;

public class KeycloakConfiguration
{
  public string BaseUrl { get; set; } = string.Empty; // e.g., "http://localhost:8080"
  public string Audience { get; set; } = string.Empty;
  public bool Https { get; set; }
  public string Secret { get; set; } = string.Empty;


  // TODO@JOREN: do we want this in shared?
  public string AdminRealm { get; set; } = string.Empty; // For admin operations
  public string AdminClientId { get; set; } = string.Empty;
  public string AdminUsername { get; set; } = string.Empty;
  public string AdminPassword { get; set; } = string.Empty;
}
