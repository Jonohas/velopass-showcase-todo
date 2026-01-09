using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Keycloak;

/// <summary>
/// #################################################
/// VERY MUCH NOT TESTED YET, JUST A PROOF OF CONCEPT
/// #################################################
/// </summary>
public interface IKeycloakAdminService
{
  Task<bool> CreateRealmAsync(string realmName, string tenantName);
  Task<bool> RealmExistsAsync(string realmName);
}

public class KeycloakAdminService : IKeycloakAdminService
{
  private readonly HttpClient _httpClient;
  private readonly KeycloakConfiguration _configuration;
  private readonly ILogger<KeycloakAdminService> _logger;

  public KeycloakAdminService(
    HttpClient httpClient,
    IOptions<KeycloakConfiguration> configuration,
    ILogger<KeycloakAdminService> logger)
  {
    _httpClient = httpClient;
    _configuration = configuration.Value;
    _logger = logger;
  }

  public async Task<bool> CreateRealmAsync(string realmName, string tenantName)
  {
    try
    {
      var accessToken = await GetAdminAccessTokenAsync();
      if (string.IsNullOrEmpty(accessToken))
      {
        return false;
      }

      dynamic realmConfig = new
      {
        realm = realmName,
        enabled = true,
        displayName = $"{tenantName} Realm",
        loginTheme = "base",
        accountTheme = "base",
        adminTheme = "base",
        emailTheme = "base",
        sslRequired = "none", // For development
        registrationAllowed = true,
        loginWithEmailAllowed = true,
        duplicateEmailsAllowed = false,
        resetPasswordAllowed = true,
        editUsernameAllowed = true,
        bruteForceProtected = true,
        accessTokenLifespan = 900,
        clients = new object[]
        {
          new
          {
            clientId = "react-frontend",
            enabled = true,
            protocol = "openid-connect",
            publicClient = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            directAccessGrantsEnabled = false,
            redirectUris = new[] { "http://localhost:3000/*" },
            webOrigins = new[] { "http://localhost:3000" },
            attributes = new Dictionary<string, string>
            {
              { "pkce.code.challenge.method", "S256" }
            }
          },
          new
          {
            clientId = "dotnet-backend",
            enabled = true,
            protocol = "openid-connect",
            publicClient = false,
            secret = _configuration.Secret,
            serviceAccountsEnabled = true,
            standardFlowEnabled = false
          }
        }
      };

      var json = JsonSerializer.Serialize(realmConfig);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

      var response = await _httpClient.PostAsync(
        $"{_configuration.BaseUrl}/admin/realms",
        content);

      return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Failed to create realm {realmName}");
      return false;
    }
  }

  public async Task<bool> RealmExistsAsync(string realmName)
  {
    try
    {
      var accessToken = await GetAdminAccessTokenAsync();
      if (string.IsNullOrEmpty(accessToken))
      {
        return false;
      }

      _httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", accessToken);

      var response = await _httpClient.GetAsync(
        $"{_configuration.BaseUrl}/admin/realms/{realmName}");

      return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Failed to check if realm {realmName} exists");
      return false;
    }
  }

  private async Task<string?> GetAdminAccessTokenAsync()
  {
    try
    {
      var tokenRequest = new Dictionary<string, string>
      {
        { "grant_type", "password" },
        { "client_id", _configuration.AdminClientId },
        { "username", _configuration.AdminUsername },
        { "password", _configuration.AdminPassword }
      };

      var content = new FormUrlEncodedContent(tokenRequest);
      var response = await _httpClient.PostAsync(
        $"{_configuration.BaseUrl}/realms/{_configuration.AdminRealm}/protocol/openid-connect/token",
        content);

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        return tokenResponse.GetProperty("access_token").GetString();
      }

      return null;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to get admin access token");
      return null;
    }
  }
}
