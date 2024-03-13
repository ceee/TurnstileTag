using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TurnstileTag;

public static class TurnstileServiceCollectionExtensions
{
  public static IServiceCollection AddTurnstile(this IServiceCollection services)
  {
    services.TryAddSingleton<ITurnstile, Turnstile>();
    services.TryAddSingleton<ValidateTurnstileAuthorizationFilter>();
    services.AddOptions<TurnstileOptions>().BindConfiguration("Turnstile");
    return services;
  }


  public static IServiceCollection AddTurnstile(this IServiceCollection services, Action<TurnstileOptions> configure)
  {
    services.TryAddSingleton<ITurnstile, Turnstile>();
    services.TryAddSingleton<ValidateTurnstileAuthorizationFilter>();
    services.AddOptions<TurnstileOptions>().BindConfiguration("Turnstile");
    services.PostConfigure(configure);
    return services;
  }


  public static IServiceCollection AddTurnstile(this IServiceCollection services, string configName, IConfiguration namedConfigurationSection)
  {
    services.TryAddSingleton<ITurnstile, Turnstile>();
    services.TryAddSingleton<ValidateTurnstileAuthorizationFilter>();
    services.AddOptions<TurnstileOptions>(configName).Bind(namedConfigurationSection);
    return services;
  }
}
