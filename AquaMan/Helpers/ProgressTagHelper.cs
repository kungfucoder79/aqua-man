using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquaMan.Helpers
{
    [HtmlTargetElement("progressbar")]
    public class ProgressTagHelper : TagHelper
    {
        public double WaterHeight { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "progress-bar progress-bar-success progress-bar-striped");
            output.Attributes.SetAttribute("style", "width: " + WaterHeight + "%");
        }
    }

}
