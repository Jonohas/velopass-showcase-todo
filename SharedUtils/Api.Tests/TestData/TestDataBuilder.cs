using Bogus;

namespace Shared.Api.Tests.TestData;

/// <summary>
/// Base class for creating test data builders using Bogus
/// </summary>
/// <typeparam name="T">The type of entity to build</typeparam>
public abstract class TestDataBuilder<T> where T : class
{
    protected Faker<T> Faker { get; set; }

    protected TestDataBuilder()
    {
        Faker = new Faker<T>();
        ConfigureRules();
    }

    /// <summary>
    /// Configure the Faker rules for generating test data
    /// </summary>
    protected abstract void ConfigureRules();

    /// <summary>
    /// Generates a single instance
    /// </summary>
    public virtual T Build()
    {
        return Faker.Generate();
    }

    /// <summary>
    /// Generates multiple instances
    /// </summary>
    public virtual List<T> Build(int count)
    {
        return Faker.Generate(count);
    }

    /// <summary>
    /// Sets a specific seed for reproducible random data
    /// </summary>
    public TestDataBuilder<T> WithSeed(int seed)
    {
        Randomizer.Seed = new Random(seed);
        return this;
    }
}

/// <summary>
/// Utility class for generating common test data
/// </summary>
public static class TestData
{
    private static readonly Faker _faker = new();

    public static string RandomString(int length = 10) => _faker.Random.String2(length);

    public static string RandomEmail() => _faker.Internet.Email();

    public static string RandomUsername() => _faker.Internet.UserName();

    public static string RandomPassword() => _faker.Internet.Password();

    public static string RandomUrl() => _faker.Internet.Url();

    public static int RandomInt(int min = 1, int max = 1000) => _faker.Random.Int(min, max);

    public static Guid RandomGuid() => _faker.Random.Guid();

    public static DateTime RandomDate() => _faker.Date.Past();

    public static DateTime RandomFutureDate() => _faker.Date.Future();

    public static bool RandomBool() => _faker.Random.Bool();

    public static string RandomPhoneNumber() => _faker.Phone.PhoneNumber();

    public static string RandomCompanyName() => _faker.Company.CompanyName();

    public static string RandomFirstName() => _faker.Name.FirstName();

    public static string RandomLastName() => _faker.Name.LastName();

    public static string RandomFullName() => _faker.Name.FullName();

    /// <summary>
    /// Creates a unique identifier for test isolation
    /// </summary>
    public static string UniqueIdentifier() => $"test-{Guid.NewGuid():N}";

    /// <summary>
    /// Creates a tenant identifier for multi-tenant tests
    /// </summary>
    public static string TenantIdentifier(string? prefix = null) =>
        prefix != null ? $"{prefix}-{Guid.NewGuid():N}" : $"tenant-{Guid.NewGuid():N}";
}
