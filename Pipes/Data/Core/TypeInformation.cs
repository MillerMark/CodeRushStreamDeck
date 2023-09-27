#nullable enable
namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class TypeInformation
    {
        public TypeKind Kind { get; set; }
        public string? SimpleType { get; set; }
        public string? GenericType { get; set; }
        public string? TypeParam1 { get; set; }
        public string? TypeParam2 { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(SimpleType) && string.IsNullOrWhiteSpace(GenericType);
        }

        public TypeInformation()
        {

        }

        public override string? ToString()
        {
            switch(Kind)
            {
                case TypeKind.Simple:
                    return SimpleType;

                case TypeKind.GenericOneTypeParameter:
                    return $"{GenericType}<{TypeParam1}>";

                case TypeKind.GenericTwoTypeParameters:
                    return $"{GenericType}<{TypeParam1}, {TypeParam2}>";
            }
            return base.ToString();
        }
    }
}
