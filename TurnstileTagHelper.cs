using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace TurnstileTag;

[HtmlTargetElement("cf-turnstile-input", TagStructure = TagStructure.NormalOrSelfClosing)]
public class TurnstileTagHelper : TagHelper
{
  readonly IOptions<TurnstileOptions> _options;


  /// <summary>
  /// Gets the <see cref="Rendering.ViewContext"/> of the executing view.
  /// </summary>
  [HtmlAttributeNotBound]
  [ViewContext]
  public ViewContext? ViewContext { get; set; }


  public TurnstileTagHelper(IOptions<TurnstileOptions> options)
  {
    _options = options;
  }


  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    output.TagMode = TagMode.StartTagAndEndTag;
    output.TagName = "div";
    output.Attributes.Add("class", "cf-turnstile");
    output.Attributes.Add("data-sitekey", _options.Value.PublicKey);
    ViewContext!.ViewData.TryAdd("turnstile", true);
  }
}