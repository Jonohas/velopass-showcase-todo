using System.Diagnostics;
using JV.ResultUtilities.Exceptions;
using JV.ResultUtilities.ValidationMessage;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Translations;

namespace Shared.Api.ResultTypeUtils.ExceptionHandlers;

public sealed class ResultExceptionHandler(
  IProblemDetailsService problemDetailsService,
  ITranslator translator,
  ILogger<ResultExceptionHandler> logger)
  : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
    CancellationToken cancellationToken)
  {
    httpContext.Response.StatusCode = exception switch
    {
      ApplicationException => StatusCodes.Status400BadRequest,
      _ => StatusCodes.Status500InternalServerError
    };

    httpContext.Response.ContentType = "application/problem+json";

    var problemDetails = new ProblemDetails
    {
      Type = exception.GetType().Name,
      Title = translator.Translate("AnErrorOccured"),
      Detail = exception
        .Message, // TODO@JOREN  dangerous to use in production, could possibly be exploited to see details about system
      // Detail = _translator.Translate(TranslationKeys.ContactDeveloper),
    };

    if (exception is ResultException resultException)
    {
      logger.LogError(string.Join(", ", resultException.ValidationMessages.Select(vm => vm.MapToErrorMessage())));
      problemDetails.Detail =
        string.Join(", ", resultException.ValidationMessages.Select(TranslateValidationMessage));
    }

    // Ensure the problem details include a traceId that matches Seq/OpenTelemetry (32-hex Activity.TraceId)
    var traceId = Activity.Current?.TraceId.ToString();
    problemDetails.Extensions["traceId"] = string.IsNullOrWhiteSpace(traceId) ? httpContext.TraceIdentifier : traceId;

    return await problemDetailsService.TryWriteAsync(
      new ProblemDetailsContext()
      {
        HttpContext = httpContext,
        Exception = exception,
        ProblemDetails = problemDetails
      });
  }

  private string TranslateValidationMessage(ValidationMessage validationMessage)
  {
    var key = validationMessage.TranslationKey;
    var translationResult = translator.Translate(key, validationMessage.Parameters.ToArray<object>());

    return string.IsNullOrEmpty(translationResult) ? key : translationResult;
  }
}
