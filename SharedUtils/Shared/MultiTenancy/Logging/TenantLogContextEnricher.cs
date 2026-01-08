// TODO@JOREN create shared.multiTenancy.logging project for this

using Serilog.Context;

namespace Shared.MultiTenancy.Logging;

public interface ILogContextEnricher<TTenant> : ITenantResolvedListener<TTenant> where TTenant : ITenant {}

public sealed class TenantIdNameLogContextEnricher<TTenant> : ILogContextEnricher<TTenant>
  where TTenant : ITenant
{
  public Task OnTenantResolvedAsync(TTenant tenant, CancellationToken cancellationToken)
  {
    LogContext.PushProperty("TenantId", tenant.Id);
    LogContext.PushProperty("TenantName", tenant.Name);
    return Task.CompletedTask;
  }
}
