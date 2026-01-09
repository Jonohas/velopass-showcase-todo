using System.Globalization;
using Microsoft.Extensions.Localization;

namespace Shared.Translations;

public class Translations
{
}

public class DefaultTranslator(IStringLocalizer<Translations> localizer) : ITranslator
{
  public string Translate(string key)
  {
    return Translate(key, string.Empty);
  }

  public string Translate(string key, params object[] parameters)
  {
    var result = localizer.GetString(key, parameters);

    if (result.ResourceNotFound)
    {
      var originalCulture = CultureInfo.CurrentUICulture;
      try
      {
        CultureInfo.CurrentUICulture = Culture.Default;
        var fallbackResult = localizer.GetString(key, parameters);

        return fallbackResult.ResourceNotFound ? key : fallbackResult.Value;
      }
      finally
      {
        CultureInfo.CurrentUICulture = originalCulture;
      }
    }

    return result.Value;
  }
}
