using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.TestHost;

namespace Shared.Api.Tests.Extensions;

/// <summary>
/// Extension methods for HttpClient to simplify E2E API testing
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Adds authentication claims to the request
    /// </summary>
    public static HttpClient WithAuthentication(this HttpClient client, List<Claim> claims)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");

        // Store claims in a way that can be accessed by TestAuthenticationHandler
        // This is done through the TestServer's SendAsync override
        return client;
    }

    /// <summary>
    /// Adds tenant header to the request
    /// </summary>
    public static HttpClient WithTenant(this HttpClient client, string tenantIdentifier)
    {
        client.DefaultRequestHeaders.Add("Tenant", tenantIdentifier);
        return client;
    }

    /// <summary>
    /// Adds a custom header to the request
    /// </summary>
    public static HttpClient WithHeader(this HttpClient client, string name, string value)
    {
        client.DefaultRequestHeaders.Add(name, value);
        return client;
    }

    /// <summary>
    /// Creates an authenticated GET request
    /// </summary>
    public static async Task<HttpResponseMessage> GetAuthenticatedAsync(
        this HttpClient client,
        string requestUri,
        List<Claim> claims)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.SetTestClaims(claims);
        return await client.SendAsync(request);
    }

    /// <summary>
    /// Creates an authenticated POST request
    /// </summary>
    public static async Task<HttpResponseMessage> PostAuthenticatedAsync<T>(
        this HttpClient client,
        string requestUri,
        T content,
        List<Claim> claims)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(content, options: JsonOptions)
        };
        request.SetTestClaims(claims);
        return await client.SendAsync(request);
    }

    /// <summary>
    /// Creates an authenticated PUT request
    /// </summary>
    public static async Task<HttpResponseMessage> PutAuthenticatedAsync<T>(
        this HttpClient client,
        string requestUri,
        T content,
        List<Claim> claims)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(content, options: JsonOptions)
        };
        request.SetTestClaims(claims);
        return await client.SendAsync(request);
    }

    /// <summary>
    /// Creates an authenticated DELETE request
    /// </summary>
    public static async Task<HttpResponseMessage> DeleteAuthenticatedAsync(
        this HttpClient client,
        string requestUri,
        List<Claim> claims)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
        request.SetTestClaims(claims);
        return await client.SendAsync(request);
    }

    /// <summary>
    /// Creates an authenticated request with tenant context
    /// </summary>
    public static async Task<HttpResponseMessage> GetAuthenticatedWithTenantAsync(
        this HttpClient client,
        string requestUri,
        List<Claim> claims,
        string tenantIdentifier)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("Tenant", tenantIdentifier);
        request.SetTestClaims(claims);
        return await client.SendAsync(request);
    }

    /// <summary>
    /// Reads the response as JSON and deserializes it
    /// </summary>
    public static async Task<T?> ReadAsJsonAsync<T>(this HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    /// <summary>
    /// Sets test claims on the request for authentication
    /// </summary>
    private static void SetTestClaims(this HttpRequestMessage request, List<Claim> claims)
    {
        request.Options.TryAdd("TestClaims", claims);
    }
}

/// <summary>
/// Extension methods for HttpRequestMessage in tests
/// </summary>
public static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Adds tenant header to a specific request
    /// </summary>
    public static HttpRequestMessage WithTenant(this HttpRequestMessage request, string tenantIdentifier)
    {
        request.Headers.Add("Tenant", tenantIdentifier);
        return request;
    }

    /// <summary>
    /// Adds authentication claims to a specific request
    /// </summary>
    public static HttpRequestMessage WithClaims(this HttpRequestMessage request, List<Claim> claims)
    {
        request.Options.TryAdd("TestClaims", claims);
        return request;
    }
}
