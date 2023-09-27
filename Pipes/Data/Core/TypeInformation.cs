#nullable enable
namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class TypeInformation
    {
        public TypeKind Kind { get; set; }
        public string? Type { get; set; }
        public string? TypeParam1 { get; set; }
        public string? TypeParam2 { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Type);
        }

        public TypeInformation()
        {

        }

        public override string? ToString()
        {
            switch(Kind)
            {
                case TypeKind.Simple:
                    return Type;
                case TypeKind.GenericOneTypeParameter:
                    return $"{Type}<{TypeParam1}>";
                case TypeKind.GenericTwoTypeParameters:
                    return $"{Type}<{TypeParam1}, {TypeParam2}>";
            }
            return base.ToString();
        }
    }
}
