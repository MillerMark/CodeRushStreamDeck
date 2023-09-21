using System.Collections.Generic;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class CodeRushTemplateCommandData : ButtonStreamDeckData
    {
        public string TemplateName { get; set; }
        public string VariablesToSet { get; set; }
        public List<DynamicListEntry> DynamicListEntries { get; set; }
        public string Context { get; set; }
        public CodeRushTemplateCommandData()
        {

        }
    }
}
