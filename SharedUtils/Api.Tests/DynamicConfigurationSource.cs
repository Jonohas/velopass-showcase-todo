using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Shared.Api.Tests;

public sealed class DynamicDictionaryConfigurationSource : IConfigurationSource
{
  private readonly IDictionary<string, string?> _data;
  private readonly ConfigurationReloadToken _reloadToken;

  public DynamicDictionaryConfigurationSource(
    IDictionary<string, string?> data,
    ConfigurationReloadToken reloadToken)
  {
    _data = data;
    _reloadToken = reloadToken;
  }

  public IConfigurationProvider Build(IConfigurationBuilder builder)
    => new DynamicDictionaryConfigurationProvider(_data, _reloadToken);
}

public sealed class DynamicDictionaryConfigurationProvider : ConfigurationProvider
{
  private readonly IDictionary<string, string?> _data;
  private readonly ConfigurationReloadToken _reloadToken;

  public DynamicDictionaryConfigurationProvider(
    IDictionary<string, string?> data,
    ConfigurationReloadToken reloadToken)
  {
    _data = data;
    _reloadToken = reloadToken;
    Data = new Dictionary<string, string?>(_data);
  }

  public override bool TryGet(string key, out string? value)
    => _data.TryGetValue(key, out value);

  public override void Set(string key, string? value)
  {
    _data[key] = value;
    Data[key] = value;
    OnReload();
  }

  public new IChangeToken GetReloadToken() => _reloadToken;

  public override void Load()
  {
    Data = new Dictionary<string, string?>(_data);
  }
}