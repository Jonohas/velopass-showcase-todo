using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;

namespace Shared.Logging;

public static class SerilogInstaller
{
  public static ConfigureHostBuilder ConfigureSerilog(this ConfigureHostBuilder hostBuilder,
    SerilogConfiguration config)
  {
    hostBuilder.UseSerilog((_, configuration) =>
    {
      configuration
        .Enrich.FromLogContext()
        .Enrich.WithExceptionDetails() // capture exception details
        .Enrich.WithSpan()
        .WriteTo.Console(
          outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

      if (config.EnableFileLogging)
      {
        configuration
          .WriteTo.File("Logs/log-.log",
            rollingInterval: RollingInterval.Hour,
            retainedFileCountLimit: 14,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1));
      }

      // Seq sink (optional): enable when Seq URL provided via configuration or environment
      var seqUrl = config.SeqUrl ?? Environment.GetEnvironmentVariable("SEQ_SERVER_URL");
      if (!string.IsNullOrWhiteSpace(seqUrl))
      {
        var seqApiKey = config.SeqApiKey ?? Environment.GetEnvironmentVariable("SEQ_API_KEY");
        configuration.WriteTo.Seq(serverUrl: seqUrl!, apiKey: string.IsNullOrWhiteSpace(seqApiKey) ? null : seqApiKey);
      }
    });

    return hostBuilder;
  }
}
