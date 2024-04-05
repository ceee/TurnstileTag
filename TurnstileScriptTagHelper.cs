using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace TurnstileTag;

[HtmlTargetElement("cf-turnstile-script", TagStructure = TagStructure.NormalOrSelfClosing)]
public class TurnstileScriptTagHelper : TagHelper
{
  /// <summary>
  /// Force rendering of script tag.
  /// Otherwise it is only rendered when a turnstile input was rendered.
  /// </summary>
  public bool Force { get; set; }

  readonly IOptions<TurnstileOptions> _options;

  /// <summary>
  /// Gets the <see cref="Rendering.ViewContext"/> of the executing view.
  /// </summary>
  [HtmlAttributeNotBound]
  [ViewContext]
  public ViewContext? ViewContext { get; set; }


  public TurnstileScriptTagHelper(IOptions<TurnstileOptions> options)
  {
    _options = options;
  }


  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    if (Force || (ViewContext != null && ViewContext.ViewData.ContainsKey("turnstile")))
    {
      output.TagMode = TagMode.StartTagAndEndTag;
      output.TagName = "script";
      output.Attributes.Add(new TagHelperAttribute("async"));
      output.Attributes.Add(new TagHelperAttribute("defer"));
      output.Attributes.Add("src", _options.Value.ApiUrl + "/api.js");
    }
    else 
    {
      output.SuppressOutput();
    }
  }
}