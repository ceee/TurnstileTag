namespace TurnstileTag;

public class TurnstileOptions
{
  public string? PublicKey { get; set; }

  public string? SecretKey { get; set; }

  public string ApiUrl { get; set; } = "https://challenges.cloudflare.com/turnstile/v0";

  public string FormFieldName { get; set; } = "cf-turnstile-response";
}