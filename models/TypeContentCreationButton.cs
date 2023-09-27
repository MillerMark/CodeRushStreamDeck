using DevExpress.CodeRush.Foundation.Pipes.Data;

namespace CodeRushStreamDeck.Models
{
    public class TypeContentCreationButton
    {
        public string TemplateToExpand { get; set; }
        public string Context { get; set; }
        public string FullTemplateName { get; set; }
        public string LastTypeRecognized { get; set; }
        public TypeKind Kind { get; set; }
        public string SimpleType { get; set; }
        public string GenericType { get; set; }
        public string TypeParam1 { get; set; }
        public string TypeParam2 { get; set; }
    }
}
