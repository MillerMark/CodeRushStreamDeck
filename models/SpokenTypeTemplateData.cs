using System;
using System.Runtime.Versioning;
using System.Collections.Generic;
using DevExpress.CodeRush.Foundation.Speak.Types;

namespace CodeRushStreamDeck.Models
{
    [SupportedOSPlatform("windows")]
    public class SpokenTypeTemplateData : ICanAddTextLines
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

        public int LineCount
        {
            get
            {
                switch (Kind)
                {
                    case TypeKind.Simple:
                        return 1;
                    case TypeKind.GenericOneTypeParameter:
                        return 2;
                    case TypeKind.GenericTwoTypeParameters:
                        return 3;
                }
                return 0;
            }
        }

        string GetOnlyTypeName(string typeName)
        {
            int lastIndexOfDot = typeName.LastIndexOf('.');
            if (lastIndexOfDot < typeName.Length - 1)
                return typeName.Substring(lastIndexOfDot + 1);
            return typeName;
        }

        public void AddTextLines(List<TextLine> textLines, float x, float line1, float line2, float line3)
        {
            switch (Kind)
            {
                case TypeKind.Simple:
                    textLines.Add(new TextLine() { Text = GetOnlyTypeName(SimpleType), X = x, Y = line3 });
                    break;
                case TypeKind.GenericOneTypeParameter:
                    textLines.Add(new TextLine() { Text = GetOnlyTypeName(GenericType), X = x, Y = line2 });
                    textLines.Add(new TextLine() { Text = $"<{GetOnlyTypeName(TypeParam1)}>", X = x, Y = line3 });
                    break;
                case TypeKind.GenericTwoTypeParameters:
                    textLines.Add(new TextLine() { Text = GetOnlyTypeName(GenericType), X = x, Y = line1 });
                    textLines.Add(new TextLine() { Text = $"<{GetOnlyTypeName(TypeParam1)}, ", X = x, Y = line2 });
                    textLines.Add(new TextLine() { Text = $"{GetOnlyTypeName(TypeParam2)}>", X = x, Y = line3 });
                    break;
            }
        }

        public string GetSimpleText()
        {
            return GetOnlyTypeName(SimpleType);
        }
    }
}
