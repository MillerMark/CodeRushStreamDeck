using System.Collections.Generic;

namespace CodeRushStreamDeck.Models
{
    public class VisualStudioCommandModel: ICanSupportImageCarousel
    {
        public string EvtTargetValue { get; set; } = string.Empty;
        public string EvtValue { get; set; } = string.Empty;
        public string OverrideCommand { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string SearchTextForImages { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public string SelectedImage { get; set; } = string.Empty;
    }
}
