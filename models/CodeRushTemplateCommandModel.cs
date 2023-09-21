namespace CodeRushStreamDeck.Models
{
    public class CodeRushTemplateCommandModel
    {
        public string TemplateToExpand { get; set; } = string.Empty;
        public string VariablesToSet { get; set; } = string.Empty;
        public string FullTemplateName { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
    }
}
