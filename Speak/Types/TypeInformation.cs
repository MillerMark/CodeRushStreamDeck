#nullable enable
using System;
using Microsoft.CodeAnalysis;

namespace DevExpress.CodeRush.Foundation.Speak.Types {
    public class TypeInformation {
        public TypeKind Kind { get; set; }
        public string? SimpleType { get; set; }
        public string? GenericType { get; set; }
        public string? TypeParam1 { get; set; }
        public string? TypeParam2 { get; set; }
        public bool IsArray { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(SimpleType) && string.IsNullOrWhiteSpace(GenericType);
        }

#if !CodeRushStreamDeck
        string GetArrayStr(string language) {
            if (IsArray) {
                if(language == LanguageNames.CSharp)
                    return "[]";
                if(language == LanguageNames.VisualBasic)
                    return "()";
            }

            return string.Empty;
        }

        private ITypeSymbol? typeSymbol;

        public TypeInformation(ITypeSymbol? typeSymbol) {
            this.typeSymbol = typeSymbol;
        }

        public string GetTypeString(SemanticModel semantic, int position) {
            if(typeSymbol != null)
                return typeSymbol.ToMinimalDisplayString(semantic, position, SymbolDisplayFormat.MinimallyQualifiedFormat) + GetArrayStr(semantic.Language);

            switch(Kind) {
                case TypeKind.Simple:
                    return SimpleType + GetArrayStr(semantic.Language);

                case TypeKind.GenericOneTypeParameter:
                    if(semantic.Language == LanguageNames.CSharp)
                        return $"{GenericType}<{TypeParam1}>" + GetArrayStr(semantic.Language);
                    if(semantic.Language == LanguageNames.VisualBasic)
                        return $"{GenericType}(Of {TypeParam1})" + GetArrayStr(semantic.Language);
                    return string.Empty;

                case TypeKind.GenericTwoTypeParameters:
                    if(semantic.Language == LanguageNames.CSharp)
                        return $"{GenericType}<{TypeParam1}, {TypeParam2}>" + GetArrayStr(semantic.Language);
                    if(semantic.Language == LanguageNames.VisualBasic)
                        return $"{GenericType}(Of {TypeParam1}, {TypeParam2})" + GetArrayStr(semantic.Language);
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }
#endif
    }
}
