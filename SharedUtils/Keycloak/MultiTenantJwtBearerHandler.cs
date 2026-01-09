using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using JV.ResultUtilities.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Shared.Api;
using Shared.Caching;
using Shared.Keycloak.User;
using Shared.MultiTenancy;
using Shared.Permissions.Repositories;
// added for endpoint metadata inspection

// added for GetEndpoint extension

namespace Shared.Keycloak;

internal static class ConfigurationManagerCacheKeys
{
  public const string Base = "ConfigurationManager:";
  public const string GetAll = Base + "GetAll";

  public static string GetTokenValidationParametersByRealmName(string name) =>
    Base + $"GetTokenValidationParametersByRealmName:{name}";
}

public class MultiTenantJwtBearerHandler<TTenant, TUser> : JwtBearerHandler
  where TTenant : ITenant
  where TUser : BaseUser
{
  private readonly ITenantContext<TTenant> _tenantContext;
  private readonly IRequestTypeContext _requestTypeContext;
  private readonly KeycloakConfiguration _configuration;
  private readonly ILogger<MultiTenantJwtBearerHandler<TTenant, TUser>> _logger;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly HybridCache _cache;
  private readonly IGroupMembershipRepository _groupMembershipRepository;
  private readonly IUserService<TUser> _userService;

  public MultiTenantJwtBearerHandler(
    IOptionsMonitor<JwtBearerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ITenantContext<TTenant> tenantContext,
    KeycloakConfiguration configuration,
    IRequestTypeContext requestTypeContext,
    IHttpClientFactory httpClientFactory,
    HybridCache cache,
    IGroupMembershipRepository groupMembershipRepository, IUserService<TUser> userService)
    : base(options, logger, encoder)
  {
    _tenantContext = tenantContext;
    _requestTypeContext = requestTypeContext;
    _httpClientFactory = httpClientFactory;
    _cache = cache;
    _groupMembershipRepository = groupMembershipRepository;
    _userService = userService;
    _configuration = configuration;
    _logger = logger.CreateLogger<MultiTenantJwtBearerHandler<TTenant, TUser>>();
  }

  private Task<bool> RequiresAuthenticationAsync()
  {
    var endpoint = Context.GetEndpoint();
    if (endpoint != null && (endpoint.Metadata.GetMetadata<IAllowAnonymous>() != null || endpoint.Metadata.Count == 0))
    {
      _logger.LogDebug("Skipping JWT authentication for AllowAnonymous endpoint: {Endpoint}", endpoint.DisplayName);
      return Task.FromResult(false);
    }

    return Task.FromResult(true);
  }

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
  {
    if (!_requestTypeContext.IsApiRequest)
      return await base.HandleAuthenticateAsync();

    // path-based skip for health and readiness probes
    var path = Request.Path;

    // Skip processing for endpoints that do not require authentication (e.g. [AllowAnonymous])
    if (!await RequiresAuthenticationAsync())
      return AuthenticateResult.NoResult();

    try
    {
      var realmName = _tenantContext.Tenant.RealmName;
      if (string.IsNullOrWhiteSpace(realmName))
      {
        _logger.LogWarning("No realm found for tenant");
        return AuthenticateResult.Fail("No realm configured for tenant");
      }

      // Extract the token from the request
      var token = GetToken();
      if (string.IsNullOrEmpty(token))
      {
        _logger.LogWarning("No token found in request");
        return AuthenticateResult.Fail("No token provided");
      }

      // Validate the token belongs to the correct realm
      var tokenValidationResult = await ValidateTokenForRealmAsync(token, realmName);
      if (!tokenValidationResult.IsValid)
      {
        _logger.LogWarning("Token validation failed: {Error}", tokenValidationResult.Error);
        return AuthenticateResult.Fail(tokenValidationResult.Error ?? "Token validation failed");
      }

      // Create an authentication ticket with validated claims (including processed roles)
      var identity = new ClaimsIdentity(tokenValidationResult.Claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, Scheme.Name);

      _logger.LogInformation("Successfully authenticated user for realm: {RealmName}", realmName);
      return AuthenticateResult.Success(ticket);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in multi-tenant JWT authentication");
      return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
    }
  }

  private string? GetToken()
  {
    var authorization = Request.Headers.Authorization.ToString();
    if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
      return authorization["Bearer ".Length..].Trim();
    }

    // Check for token in query parameters (fallback)
    if (Request.Query.TryGetValue("access_token", out var token))
    {
      return token.ToString();
    }

    return null;
  }

  private ConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(string configUrl)
  {
    var httpClient = _httpClientFactory.CreateClient();

    _logger.LogDebug("Creating configuration manager for URL: {ConfigUrl}, RequireHttps: {RequireHttps}",
      configUrl, _configuration.Https);

    return new ConfigurationManager<OpenIdConnectConfiguration>(
      configUrl,
      new OpenIdConnectConfigurationRetriever(),
      new HttpDocumentRetriever(httpClient)
      {
        RequireHttps = _configuration.Https
      }
    );
  }

  private async Task<TokenValidationParameters> GetConfigurationAsync(string expectedIssuer)
  {
    var configurationUrl = $"{expectedIssuer}/.well-known/openid-configuration";
    var configurationManager = CreateConfigurationManager(configurationUrl);
    var openIdConfig = await configurationManager.GetConfigurationAsync(Context.RequestAborted);
    return new TokenValidationParameters
    {
      ValidateIssuer = true,
      ValidateAudience = true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ClockSkew = TimeSpan.Zero,

      ValidIssuer = expectedIssuer,
      ValidAudience = _configuration.Audience,
      IssuerSigningKeys = openIdConfig.SigningKeys,

      // Configure a role claim type for Keycloak
      RoleClaimType = ClaimTypes.Role,
      NameClaimType = "preferred_username"
    };
  }

  private async Task<TokenValidationResult> ValidateTokenForRealmAsync(string token, string realmName)
  {
    try
    {
      var expectedIssuer = $"{_configuration.BaseUrl}/realms/{realmName}";

      // Get or create a configuration manager for this realm
      var validationParameters = await _cache.GetOrCreateAsync(
        ConfigurationManagerCacheKeys.GetTokenValidationParametersByRealmName(realmName),
        async _ => await GetConfigurationAsync(expectedIssuer),
        tags: [ConfigurationManagerCacheKeys.Base],
        options: new HybridCacheEntryOptions()
        {
          Expiration = CacheTimes.Long
        },
        cancellationToken: Context.RequestAborted
      );

      // Validate the token
      var tokenHandler = new JwtSecurityTokenHandler();

      var validationResult = await tokenHandler.ValidateTokenAsync(token, validationParameters);

      if (!validationResult.IsValid)
      {
        return new TokenValidationResult
        {
          IsValid = false,
          Error = validationResult.Exception?.Message ?? "Token validation failed"
        };
      }

      // Additional validation: Check if token issuer matches expected realm
      var jwtToken = tokenHandler.ReadJwtToken(token);
      var tokenIssuer = jwtToken.Issuer;

      if (!string.Equals(tokenIssuer, expectedIssuer, StringComparison.OrdinalIgnoreCase))
      {
        return new TokenValidationResult
        {
          IsValid = false,
          Error = $"Token issuer '{tokenIssuer}' does not match expected realm '{realmName}'"
        };
      }

      var claims = validationResult.ClaimsIdentity.Claims.ToList();

      // Resolve application permissions from our database for this subject
      // Ensure we always have a subject/user id by deriving from JWT 'sub' if missing
      var subjectFromJwt = jwtToken.Subject; // 'sub' claim value
      var userId =
        validationResult.ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? validationResult.ClaimsIdentity.FindFirst("sub")?.Value
        ?? subjectFromJwt;

      // Backfill common claims if they were not mapped by the token handler
      if (!string.IsNullOrWhiteSpace(subjectFromJwt))
      {
        if (claims.All(c => c.Type != ClaimTypes.NameIdentifier))
          claims.Add(new Claim(ClaimTypes.NameIdentifier, subjectFromJwt));
        if (claims.All(c => c.Type != "sub"))
          claims.Add(new Claim("sub", subjectFromJwt));
      }

      if (!string.IsNullOrWhiteSpace(userId) && Guid.TryParse(userId, out var subjectId))
      {
        try
        {
          var permissionCodes = await _cache.GetOrCreateAsync(
            PermissionCacheKeys.GroupMemberShip.GetBySubjectId(subjectId),
            async _ => await _groupMembershipRepository.GetPermissionCodesAsync(subjectId, Context.RequestAborted),
            tags:
            [
              PermissionCacheKeys.GroupMemberShip.Base, PermissionCacheKeys.GroupMemberShip.GetBySubjectId(subjectId)
            ],
            options: new HybridCacheEntryOptions()
            {
              Expiration = CacheTimes.Long
            },
            cancellationToken: Context.RequestAborted
          );

          foreach (var code in permissionCodes)
          {
            claims.Add(new Claim(Constants.PermissionClaimType, code));
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Failed to load permissions for subject {SubjectId} in realm {RealmName}", userId,
            realmName);
        }
      }
      else
      {
        _logger.LogWarning("JWT subject claim missing or invalid GUID: {Subject}", userId);
      }

      if (!Guid.TryParse(userId, out var userIdAsGuid))
      {
        _logger.LogError("Failed to parse user id as guid: {userId}", userId);
      }

      // TODO@JOREN: add caching to the getById call

      var userExists = await _userService.UserExistsAsync(userIdAsGuid, CancellationToken.None);
      if (!userExists)
      {
        var result = await _userService.AddAsync(userIdAsGuid, CancellationToken.None);
        result.ThrowIfFailure();
      }

      _logger.LogInformation("Token successfully validated for realm: {RealmName}, User: {Username}",
        realmName,
        claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "Unknown");

      return new TokenValidationResult
      {
        IsValid = true,
        Claims = claims
      };
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Exception during token validation for realm: {RealmName}", realmName);
      return new TokenValidationResult
      {
        IsValid = false,
        Error = $"Token validation exception: {ex.Message}"
      };
    }
  }

  private class TokenValidationResult
  {
    public bool IsValid { get; init; }
    public string? Error { get; init; }
    public List<Claim> Claims { get; init; } = new();
  }
}
