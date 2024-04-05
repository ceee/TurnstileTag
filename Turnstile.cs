using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TurnstileTag;

public interface ITurnstile
{
  /// <summary>
  /// Generates the placeholder which renders the turnstile challenge.
  /// </summary>
  public IHtmlContent GetInputHtml();


  /// <summary>
  /// Generates the script tag which loads the turnstile library.
  /// </summary>
  public IHtmlContent GetScriptHtml();

  /// <summary>
  /// 
  /// </summary>
  IHtmlContent GenerateTurnstileFormHtml(ViewContext viewContext);

  /// <summary>
  /// 
  /// </summary>
  Task ValidateRequestAsync(HttpContext httpContext);

  /// <summary>
  /// 
  /// </summary>
  Task<bool> Verify(string value);
}


public class Turnstile : ITurnstile
{
  private readonly TurnstileOptions _options;


  public Turnstile(IOptions<TurnstileOptions> options)
  {
    _options = options.Value;
  }


  /// <inheritdoc />
  public async Task ValidateRequestAsync(HttpContext httpContext)
  {
    ArgumentNullException.ThrowIfNull(httpContext);

    string[] allowed = new string[] { HttpMethods.Get, HttpMethods.Head, HttpMethods.Options, HttpMethods.Trace };

    if (allowed.Contains(httpContext.Request.Method) || !httpContext.Request.HasFormContentType)
    {
      return;
    }

    IFormCollection form;

    try
    {
      form = await httpContext.Request.ReadFormAsync();
    }
    catch (Exception ex) when (ex is InvalidDataException or IOException)
    {
      // ReadFormAsync can throw InvalidDataException if the form content is malformed.
      // [or] Reading the request body (which happens as part of ReadFromAsync) may throw an exception if a client disconnects.
      // Wrap it in an AntiforgeryValidationException and allow the caller to handle it as just another antiforgery failure.
      throw new TurnstileException("Unable to read the turnstile data from the posted form.", ex);
    }

    string? captchaResponse = form[_options.FormFieldName].FirstOrDefault();

    if (string.IsNullOrEmpty(captchaResponse) || !await Verify(captchaResponse))
    {
      throw new TurnstileException("Could not verify captcha.");
    }
  }


  /// <inheritdoc />
  public async Task<bool> Verify(string value)
  {
    using HttpClient http = new();
    using HttpResponseMessage response = await http.PostAsJsonAsync(
      requestUri: new Uri(_options.ApiUrl + "/siteverify", UriKind.Absolute),
      value: new Request(_options.SecretKey!, value)
    );

    Response? model = await response.Content.ReadFromJsonAsync<Response>();
    return model != null && model.Success;
  }


  /// <inheritdoc />
  public IHtmlContent GetInputHtml()
  {
    HtmlContentBuilder builder = new();
    builder.AppendHtml($"<div class=\"cf-turnstile\" data-sitekey=\"{_options.PublicKey}\"></div>");
    return builder;
  }


  /// <inheritdoc />
  public IHtmlContent GetScriptHtml()
  {
    HtmlContentBuilder builder = new();
    builder.AppendHtml($"<script src=\"{_options.ApiUrl}/api.js\" async defer></script>");
    return builder;
  }


  /// <inheritdoc />
  public IHtmlContent GenerateTurnstileFormHtml(ViewContext viewContext)
  {
    ArgumentNullException.ThrowIfNull(viewContext);

    var formContext = viewContext.FormContext;
    if (formContext.CanRenderAtEndOfForm)
    {
      if (formContext.FormData.ContainsKey("turnstile"))
      {
        return HtmlString.Empty;
      }

      formContext.FormData.Add("turnstile", true);
    }

    viewContext.ViewData.TryAdd("turnstile", true);
    return GetInputHtml();
  }


  class Request
  {
    public string Secret { get; set; }

    public string Response { get; set; }

    public Request(string secret, string response)
    {
      Secret = secret;
      Response = response;
    }
  }

  class Response
  {
    public bool Success { get; set; }

    [JsonPropertyName("challenge_ts")]
    public DateTimeOffset ChallengedDate { get; set; }

    public string? Hostname { get; set; }

    [JsonPropertyName("error-codes")]
    public string[]? ErrorCodes { get; set; }

    public string? Action { get; set; }

    public string? Cdata { get; set; }
  }
}