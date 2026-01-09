using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Api;

public static class RequestTypeContextInstaller
{
  /// <summary>
  /// nonApiPaths is a list of paths that are not considered as api paths, for example admin paths where you do not want to apply the default RBAC
  /// </summary>
  /// <param name="services"></param>
  /// <param name="nonApiPaths"></param>
  /// <returns></returns>
  public static IServiceCollection AddRequestTypeContext(this IServiceCollection services,
    List<string>? nonApiPaths = null)
  {
    return services.AddScoped<IRequestTypeContext>(provider => new RequestTypeContext(nonApiPaths ?? []));
  }
}

public interface IRequestTypeContext
{
  bool IsApiRequest { get; set; }
  bool IsOpenApiRequest { get; set; }
  void SetRequestPath(PathString path);
  void SetRequestPath(HttpContext context);
}

public class RequestTypeContext : IRequestTypeContext
{
  private readonly List<string> _nonApiPaths;

  public RequestTypeContext(List<string> nonApiPaths)
  {
    _nonApiPaths = nonApiPaths;
  }

  private static readonly string[] OpenApiPaths = ["/openapi"];

  private bool _isApiRequest;

  bool IRequestTypeContext.IsApiRequest
  {
    get => _isApiRequest;
    set => _isApiRequest = value;
  }

  private bool _isOpenApiRequest;

  bool IRequestTypeContext.IsOpenApiRequest
  {
    get => _isOpenApiRequest;
    set => _isOpenApiRequest = value;
  }

  public void SetRequestPath(HttpContext context)
  {
    SetRequestPath(context.Request.Path);
  }

  public void SetRequestPath(PathString path)
  {
    _isApiRequest = false;
    _isOpenApiRequest = false;

    if (OpenApiPaths
        .Any(openApiPath => path
          .StartsWithSegments(openApiPath, StringComparison.OrdinalIgnoreCase)))
    {
      _isOpenApiRequest = true;
      return;
    }

    if (_nonApiPaths.Any(nonApiPath => path.StartsWithSegments(nonApiPath, StringComparison.OrdinalIgnoreCase)))
    {
      return;
    }

    _isApiRequest = true;
  }
}
