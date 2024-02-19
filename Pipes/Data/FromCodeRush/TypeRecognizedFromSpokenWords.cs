using DevExpress.CodeRush.Foundation.Speak.Types;
using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class TypeRecognizedFromSpokenWords : FromCodeRushData
    {
        public string ButtonID { get; set; }
        public TypeKind Kind { get; set; }
        public string SimpleType { get; set; }
        public string GenericType { get; set; }
        public string TypeParam1 { get; set; }
        public string TypeParam2 { get; set; }

        public TypeRecognizedFromSpokenWords()
        {
        }

        public StreamDeckData GetStreamDeckData()
        {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(TypeRecognizedFromSpokenWords) };
        }
    }
}
