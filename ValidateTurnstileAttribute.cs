using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace TurnstileTag;

/// <summary>
/// Specifies that the class or method that this attribute is applied validates the turnstile response.
/// If the turnstile response is not available or invalid, the validation will fail
/// and the action method will not execute.
/// </summary>
/// <remarks>
/// This attribute helps defend against bots and non-human requests.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ValidateTurnstileAttribute : Attribute, IFilterFactory, IOrderedFilter
{
  /// <inheritdoc />
  public int Order { get; set; } = 1100;

  /// <inheritdoc />
  public bool IsReusable => true;

  /// <inheritdoc />
  public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
  {
    return serviceProvider.GetRequiredService<ValidateTurnstileAuthorizationFilter>();
  }
}
