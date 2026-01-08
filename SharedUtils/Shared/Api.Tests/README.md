# Shared.Api.Tests - E2E Testing Framework

## Overview

This is a comprehensive, reusable End-to-End (E2E) testing framework for ASP.NET Core APIs. It provides shared infrastructure and utilities that can be used across multiple API projects in the solution.

## Features

- **WebApplicationFactory Base Class**: Easy setup for in-memory test servers
- **Mock Authentication**: Test authentication handlers that bypass real authentication
- **Database Test Helpers**: Utilities for database operations in tests
- **HTTP Client Extensions**: Fluent API for making authenticated requests
- **Test Data Builders**: Bogus-based test data generation
- **Support for Multiple Database Providers**: In-memory or real databases (PostgreSQL via Testcontainers)
- **Multi-Tenancy Support**: Built-in support for testing multi-tenant applications

## Components

### 1. ApiWebApplicationFactory

Base factory class for creating test servers with configurable options.

```csharp
public class ApiWebApplicationFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>
    where TProgram : class
    where TDbContext : DbContext
```

**Features:**
- Configurable database provider (in-memory or PostgreSQL)
- Custom service configuration via delegates
- Configuration overrides for testing
- Built-in database initialization and cleanup

**Usage:**
```csharp
var factory = new ApiWebApplicationFactory<Program, MyDbContext>(
    useInMemoryDatabase: true,
    configureTestServices: services =>
    {
        // Add custom test service configurations
        services.AddScoped<IMyService, MockMyService>();
    }
);
```

### 2. Test Authentication

#### TestAuthenticationHandler

Custom authentication handler for tests that bypasses real authentication.

```csharp
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
```

**Features:**
- Reads claims from HTTP context items
- No actual authentication logic - uses injected test claims
- Configurable scheme name

#### TestClaimsBuilder

Fluent builder for creating test user claims.

```csharp
var claims = TestClaimsBuilder.StandardUser()
    .WithUserId("user-123")
    .WithUsername("testuser")
    .WithEmail("test@example.com")
    .WithRole("Admin")
    .WithTenant("tenant-abc")
    .Build();
```

**Predefined Builders:**
- `StandardUser()`: Creates a basic authenticated user
- `AdminUser()`: Creates a user with admin role

### 3. HTTP Client Extensions

Convenient extension methods for making authenticated requests with tenant context.

```csharp
// Authenticated GET request
var response = await client.GetAuthenticatedAsync("/api/users", claims);

// Authenticated POST request
var response = await client.PostAuthenticatedAsync("/api/users", newUser, claims);

// Authenticated request with tenant
var response = await client.GetAuthenticatedWithTenantAsync(
    "/api/users",
    claims,
    "tenant-123"
);

// Deserialize response
var user = await response.ReadAsJsonAsync<User>();
```

### 4. Database Test Helpers

Utilities for managing database state in tests.

```csharp
var dbHelper = new DatabaseTestHelper<MyDbContext>(serviceProvider);

// Add test data
await dbHelper.AddEntityAsync(new User { Name = "Test User" });
await dbHelper.AddEntitiesAsync(user1, user2, user3);

// Query data
var users = await dbHelper.GetAllEntitiesAsync<User>();
var user = await dbHelper.GetEntityByIdAsync<User>(userId);

// Cleanup
await dbHelper.ClearTableAsync<User>();

// Execute custom logic
await dbHelper.ExecuteInContextAsync(async context =>
{
    var users = await context.Users.Where(u => u.IsActive).ToListAsync();
    // ... perform operations
});
```

### 5. Test Data Builders

Base class and utilities for generating realistic test data using Bogus.

```csharp
// Use static utility methods
var email = TestData.RandomEmail();
var username = TestData.RandomUsername();
var guid = TestData.RandomGuid();
var tenantId = TestData.TenantIdentifier("myapp");

// Create custom data builder
public class UserTestDataBuilder : TestDataBuilder<User>
{
    protected override void ConfigureRules()
    {
        Faker.RuleFor(u => u.Id, f => f.Random.Guid());
        Faker.RuleFor(u => u.Name, f => f.Name.FullName());
        Faker.RuleFor(u => u.Email, f => f.Internet.Email());
        Faker.RuleFor(u => u.IsActive, f => f.Random.Bool());
    }
}

// Generate test data
var user = new UserTestDataBuilder().Build();
var users = new UserTestDataBuilder().Build(10);
```

## Getting Started

### 1. Add Reference to Your Test Project

```xml
<ItemGroup>
  <ProjectReference Include="..\..\Shared\Api.Tests\Shared.Api.Tests.csproj" />
</ItemGroup>
```

### 2. Create a Custom WebApplicationFactory

```csharp
public class MyApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public MyApiWebApplicationFactory() { }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Environment"] = "Testing",
                ["Database:RunMigrationOnStartup"] = "false"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove real database
            services.RemoveAll(typeof(DbContextOptions<MyDbContext>));
            services.RemoveAll(typeof(MyDbContext));

            // Add in-memory database
            services.AddDbContext<MyDbContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
            });

            // Mock external services
            services.RemoveAll<IEmailService>();
            services.AddScoped<IEmailService, MockEmailService>();
        });

        builder.UseEnvironment("Testing");
    }
}
```

### 3. Write E2E Tests

```csharp
public class UserControllerE2ETests : IClassFixture<MyApiWebApplicationFactory>
{
    private readonly MyApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserControllerE2ETests(MyApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var claims = TestClaimsBuilder.StandardUser()
            .WithUserId("user-123")
            .Build();

        // Act
        var response = await _client.GetAuthenticatedAsync("/api/users", claims);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.ReadAsJsonAsync<List<User>>();
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUser_WithValidData_ReturnsCreated()
    {
        // Arrange
        var claims = TestClaimsBuilder.AdminUser().Build();
        var newUser = new UserTestDataBuilder().Build();

        // Act
        var response = await _client.PostAuthenticatedAsync(
            "/api/users",
            newUser,
            claims
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

## Best Practices

### 1. Use Class Fixtures for Performance

```csharp
// Good - Factory is created once per test class
public class MyTests : IClassFixture<MyApiWebApplicationFactory>
{
    private readonly MyApiWebApplicationFactory _factory;

    public MyTests(MyApiWebApplicationFactory factory)
    {
        _factory = factory;
    }
}
```

### 2. Ensure Test Isolation

- Use unique database names for in-memory databases
- Clean up test data after each test if using shared database
- Don't share state between tests

### 3. Mock External Dependencies

Always mock external services (email, SMS, payment gateways, etc.) in tests:

```csharp
builder.ConfigureTestServices(services =>
{
    services.RemoveAll<IEmailService>();
    services.AddScoped<IEmailService, MockEmailService>();
});
```

### 4. Test Important Scenarios

Focus on:
- Authentication and authorization
- Request validation
- Business logic
- Error handling
- Multi-tenancy (if applicable)

### 5. Use Descriptive Test Names

```csharp
// Good
[Fact]
public async Task CreateUser_WithInvalidEmail_ReturnsBadRequest()

// Bad
[Fact]
public async Task Test1()
```

## Dependencies

- **xUnit**: Test framework
- **FluentAssertions**: Fluent assertion library
- **Microsoft.AspNetCore.Mvc.Testing**: WebApplicationFactory support
- **Microsoft.EntityFrameworkCore.InMemory**: In-memory database provider
- **Npgsql.EntityFrameworkCore.PostgreSQL**: PostgreSQL provider (optional)
- **Testcontainers.PostgreSql**: Docker-based PostgreSQL for tests (optional)
- **Bogus**: Test data generation
- **coverlet.collector**: Code coverage collection

## Examples

See `Tenant.Api.Tests` project for a complete implementation example demonstrating:
- Custom WebApplicationFactory setup
- Authentication testing
- Multi-tenancy testing
- HTTP method testing
- Response validation

## Contributing

When adding new shared functionality:
1. Ensure it's generic and reusable
2. Add comprehensive XML documentation
3. Update this README
4. Add usage examples

## License

Internal use only - Part of the Centralis solution.
