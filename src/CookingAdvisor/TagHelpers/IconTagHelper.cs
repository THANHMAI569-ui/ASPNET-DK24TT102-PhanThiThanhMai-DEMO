using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CookingAdvisor.TagHelpers;

// Renders one symbol from the vendored Tabler sprite:
//   <icon name="clock" />  ->  <svg class="ico"><use href="/icons/sprite.svg#i-clock"/></svg>
// Icons are decorative by default; pass label="..." when the icon is the only
// content of a control and needs an accessible name.
[HtmlTargetElement("icon", TagStructure = TagStructure.WithoutEndTag)]
public class IconTagHelper : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public IconTagHelper(IUrlHelperFactory urlHelperFactory) => _urlHelperFactory = urlHelperFactory;

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; } = default!;

    [HtmlAttributeName("name")]
    public string Name { get; set; } = string.Empty;

    [HtmlAttributeName("label")]
    public string? Label { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
        var spritePath = urlHelper.Content("~/icons/sprite.svg");

        output.TagName = "svg";
        output.TagMode = TagMode.StartTagAndEndTag;

        var existingClass = output.Attributes["class"]?.Value?.ToString();
        output.Attributes.SetAttribute(
            "class", string.IsNullOrWhiteSpace(existingClass) ? "ico" : $"ico {existingClass}");

        if (string.IsNullOrWhiteSpace(Label))
        {
            output.Attributes.SetAttribute("aria-hidden", "true");
            output.Attributes.SetAttribute("focusable", "false");
        }
        else
        {
            output.Attributes.SetAttribute("role", "img");
            output.PreContent.SetHtmlContent($"<title>{System.Net.WebUtility.HtmlEncode(Label)}</title>");
        }

        output.Content.SetHtmlContent($"<use href=\"{spritePath}#i-{Name}\" />");
    }
}
