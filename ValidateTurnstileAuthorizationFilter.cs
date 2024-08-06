using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace TurnstileTag;

internal partial class ValidateTurnstileAuthorizationFilter : IAsyncAuthorizationFilter, ITurnstilePolicy
{
  readonly ITurnstile _turnstile;
  readonly ILogger _logger;

  public ValidateTurnstileAuthorizationFilter(ITurnstile turnstile, ILoggerFactory loggerFactory)
  {
    ArgumentNullException.ThrowIfNull(turnstile);

    _turnstile = turnstile;
    _logger = loggerFactory.CreateLogger(GetType());
  }

  public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
  {
    ArgumentNullException.ThrowIfNull(context);

    if (!context.IsEffectivePolicy<ITurnstilePolicy>(this))
    {
      Log.NotMostEffectiveFilter(_logger, typeof(ITurnstilePolicy));
      return;
    }

    if (!ShouldValidate(context))
    {
      return;
    }

    try
    {
      await _turnstile.ValidateRequestAsync(context.HttpContext);
    }
    catch (Exception ex)
    {
      Log.TurnstileInvalid(_logger, ex.Message, ex);
      context.Result = new AntiforgeryValidationFailedResult();
    }
  }

  protected virtual bool ShouldValidate(AuthorizationFilterContext context)
  {
    ArgumentNullException.ThrowIfNull(context);
    return true;
  }

  static partial class Log
  {
    [LoggerMessage(1, LogLevel.Information, "Turnstile validation failed. {Message}", EventName = "TurnstileInvalid")]
    public static partial void TurnstileInvalid(ILogger logger, string message, Exception exception);

    [LoggerMessage(2, LogLevel.Trace, "Skipping the execution of current filter as its not the most effective filter implementing the policy {FilterPolicy}.", EventName = "NotMostEffectiveFilter")]
    public static partial void NotMostEffectiveFilter(ILogger logger, Type filterPolicy);
  }
}
