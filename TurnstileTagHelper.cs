using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TurnstileTag;

[HtmlTargetElement("form", Attributes = "cf-turnstile")]
public class TurnstileTagHelper : TagHelper
{
  readonly ITurnstile _turnstile;

  public TurnstileTagHelper(ITurnstile turnstile)
  {
    _turnstile = turnstile;
  }

  /// <summary>
  /// Whether turnstile should be activated.
  /// </summary>
  public bool? CfTurnstile { get; set; }

  /// <summary>
  /// Gets the <see cref="Rendering.ViewContext"/> of the executing view.
  /// </summary>
  [HtmlAttributeNotBound]
  [ViewContext]
  public ViewContext? ViewContext { get; set; }


  public override void Process(TagHelperContext context, TagHelperOutput output)
  {
    if (CfTurnstile ?? false)
    {
      IHtmlContent content = _turnstile.GenerateTurnstileFormHtml(ViewContext!);
      if (content != null)
      {
        output.PostContent.AppendHtml(content);
      }
    }
  }
}