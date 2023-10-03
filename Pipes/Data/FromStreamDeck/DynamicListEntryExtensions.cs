using System;
using System.Linq;
using System.Collections.Generic;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public static class DynamicListEntryExtensions
    {

        public static string GetTemplateToExpand(this List<DynamicListEntry> dynamicListEntries, TypeKind kind, string templateToExpand, TypeInformation typeInformation)
        {
            switch (kind)
            {
                case TypeKind.Simple:
                    const string unlikelyCodeRushTypeMnemonic = "fn471";
                    dynamicListEntries.AddType(unlikelyCodeRushTypeMnemonic, typeInformation.SimpleType);
                    templateToExpand = templateToExpand.Replace("$Type$", unlikelyCodeRushTypeMnemonic);

                    break;

                case TypeKind.GenericOneTypeParameter:
                    const string unlikelyCodeRushGenericType1Mnemonic = "go293";
                    const string unlikelyType1ParamMnemonic = "du639";

                    dynamicListEntries.AddGeneric1Type(unlikelyCodeRushGenericType1Mnemonic, typeInformation.GenericType);
                    dynamicListEntries.AddType(unlikelyType1ParamMnemonic, typeInformation.TypeParam1);

                    templateToExpand = templateToExpand.Replace("$Type$", $"{unlikelyCodeRushGenericType1Mnemonic}.{unlikelyType1ParamMnemonic}");

                    break;

                case TypeKind.GenericTwoTypeParameters:
                    const string unlikelyCodeRushGenericType2Mnemonic = "ph025";
                    const string unlikelyType2Param1Mnemonic = "wy620";
                    const string unlikelyType2Param2Mnemonic = "ak629";

                    dynamicListEntries.AddGeneric2Type(unlikelyCodeRushGenericType2Mnemonic, typeInformation.GenericType);
                    dynamicListEntries.AddType(unlikelyType2Param1Mnemonic, typeInformation.TypeParam1);
                    dynamicListEntries.AddType(unlikelyType2Param2Mnemonic, typeInformation.TypeParam2);

                    templateToExpand = templateToExpand.Replace("$Type$", $"{unlikelyCodeRushGenericType2Mnemonic}.{unlikelyType2Param1Mnemonic},{unlikelyType2Param2Mnemonic}");

                    break;
            }

            return templateToExpand;
        }
        public static void AddGeneric1Type(this List<DynamicListEntry> dynamicListEntries, string mnemonic, string generic1Type)
        {
            dynamicListEntries.Add(new DynamicListEntry()
            {
                Mnemonic = mnemonic,
                ListVarName = "Generic1Type",
                Value = generic1Type
            });
        }

        public static void AddGeneric2Type(this List<DynamicListEntry> dynamicListEntries, string mnemonic, string generic2Type)
        {
            dynamicListEntries.Add(new DynamicListEntry()
            {
                Mnemonic = mnemonic,
                ListVarName = "Generic2Type",
                Value = generic2Type
            });
        }

        public static void AddType(this List<DynamicListEntry> dynamicListEntries, string mnemonic, string type)
        {
            dynamicListEntries.Add(new DynamicListEntry()
            {
                Mnemonic = mnemonic,
                ListVarName = "Type",
                Value = type
            });
        }
    }
}
