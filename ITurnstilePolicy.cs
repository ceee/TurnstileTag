using Microsoft.AspNetCore.Mvc.Filters;

namespace TurnstileTag;

/// <summary>
/// A marker interface for filters which define a policy for turnstile validation.
/// </summary>
public interface ITurnstilePolicy : IFilterMetadata
{
}
