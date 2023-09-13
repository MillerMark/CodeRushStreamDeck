namespace CodeRushStreamDeck.Models
{
    public class StringModifierModel
    {
        public string VariableName { get; set; }
        public string Value { get; set; }
        public string CurrentValue { get; set; }
        public string Color { get; set; } = "Blue";
        public bool AllowZeroSelected { get; set; } = false;
    }
}
