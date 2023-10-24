namespace CodeRushStreamDeck.Models
{
    public interface ICanSupportImageCarousel
    {
        string OverrideCommand { get; set; }
        string Command { get; set; }
        string SearchTextForImages { get; set; }
        string SelectedImage { get; set; }
        string ImageFileName { get; set; }
    }
}
