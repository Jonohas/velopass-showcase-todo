using System.Security.Claims;

namespace Shared.Api.Tests.Authentication;

/// <summary>
/// Builder for creating test claims for authenticated requests
/// </summary>
public class TestClaimsBuilder
{
    private readonly List<Claim> _claims = new();

    public TestClaimsBuilder WithUserId(string userId)
    {
        _claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        _claims.Add(new Claim("sub", userId));
        return this;
    }

    public TestClaimsBuilder WithUsername(string username)
    {
        _claims.Add(new Claim(ClaimTypes.Name, username));
        _claims.Add(new Claim("preferred_username", username));
        return this;
    }

    public TestClaimsBuilder WithEmail(string email)
    {
        _claims.Add(new Claim(ClaimTypes.Email, email));
        _claims.Add(new Claim("email", email));
        return this;
    }

    public TestClaimsBuilder WithRole(string role)
    {
        _claims.Add(new Claim(ClaimTypes.Role, role));
        return this;
    }

    public TestClaimsBuilder WithRoles(params string[] roles)
    {
        foreach (var role in roles)
        {
            _claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return this;
    }

    public TestClaimsBuilder WithPermission(string permission)
    {
        _claims.Add(new Claim("permissions", permission));
        return this;
    }

    public TestClaimsBuilder WithPermissions(params string[] permissions)
    {
        foreach (var permission in permissions)
        {
            _claims.Add(new Claim("permissions", permission));
        }
        return this;
    }

    public TestClaimsBuilder WithTenant(string tenantId)
    {
        _claims.Add(new Claim("tenant_id", tenantId));
        return this;
    }

    public TestClaimsBuilder WithClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
        return this;
    }

    public TestClaimsBuilder WithClaims(params Claim[] claims)
    {
        _claims.AddRange(claims);
        return this;
    }

    public List<Claim> Build()
    {
        return _claims;
    }

    /// <summary>
    /// Creates a standard test user with common claims
    /// </summary>
    public static TestClaimsBuilder StandardUser(
        string userId = "test-user-id",
        string username = "testuser",
        string email = "test@example.com")
    {
        return new TestClaimsBuilder()
            .WithUserId(userId)
            .WithUsername(username)
            .WithEmail(email);
    }

    /// <summary>
    /// Creates an admin test user with elevated permissions
    /// </summary>
    public static TestClaimsBuilder AdminUser(
        string userId = "admin-user-id",
        string username = "adminuser",
        string email = "admin@example.com")
    {
        return new TestClaimsBuilder()
            .WithUserId(userId)
            .WithUsername(username)
            .WithEmail(email)
            .WithRole("Admin");
    }
}
