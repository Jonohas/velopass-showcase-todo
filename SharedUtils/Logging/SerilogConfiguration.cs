namespace Shared.Logging;

public class SerilogConfiguration
{
  public bool EnableFileLogging { get; set; }
  public string? SeqUrl { get; set; }
  public string? SeqApiKey { get; set; }
}
