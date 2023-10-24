using System.Collections.Generic;
using System.Runtime.Versioning;

namespace CodeRushStreamDeck.Models
{
    [SupportedOSPlatform("windows")]
    public class CodeRushToggleStateCommandModel : CodeRushCommandModel, ICanSupportImageCarousel, ICanAddTextLines
    {
        public string Title { get; set; }
        public string StateName { get; set; }
        public string OverrideCommand { get; set; } = string.Empty;
        public string SearchTextForImages { get; set; } = string.Empty;
        public string SelectedImage { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;

        public int LineCount => 1;

        public void AddTextLines(List<TextLine> textLines, float x, float line1, float line2, float line3)
        {
            textLines.Add(new TextLine() { Text = Title, X = x, Y = line3 });
        }

        public string GetSimpleText()
        {
            return Title;
        }
    }
}
