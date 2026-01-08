using System.Net;
using System.Text.Json;

namespace Shared.MultiTenancy;

public class GenericTenantService<T> : ITenantService<T> where T : ITenant
{
  private readonly HttpClient _httpClient;
  private readonly string _adminApiBaseUrl;
  private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

  public GenericTenantService(HttpClient httpClient, string adminApiBaseUrl)
  {
    _httpClient = httpClient;
    _adminApiBaseUrl = adminApiBaseUrl.TrimEnd('/');
  }

  public T? GetByName(string name) => GetByNameAsync(name, CancellationToken.None).GetAwaiter().GetResult();
  public async Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken)
  {
    var response = await _httpClient.GetAsync($"{_adminApiBaseUrl}/tenants/by-name/{Uri.EscapeDataString(name)}", cancellationToken);
    if (response.StatusCode == HttpStatusCode.NotFound) return default;
    response.EnsureSuccessStatusCode();
    var json = await response.Content.ReadAsStringAsync(cancellationToken);
    return JsonSerializer.Deserialize<T>(json, _jsonOptions);
  }

  public T? GetById(Guid id) => GetByIdAsync(id, CancellationToken.None).GetAwaiter().GetResult();
  public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
  {
    var response = await _httpClient.GetAsync($"{_adminApiBaseUrl}/tenants/by-id/{id}", cancellationToken);
    if (response.StatusCode == HttpStatusCode.NotFound) return default;
    response.EnsureSuccessStatusCode();
    var json = await response.Content.ReadAsStringAsync(cancellationToken);
    return JsonSerializer.Deserialize<T>(json, _jsonOptions);
  }
}

