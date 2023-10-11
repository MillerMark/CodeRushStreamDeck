using System.Collections.Generic;
using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class CodeRushStateUpdate : FromCodeRushData
    {
        public Dictionary<string, bool> State { get; set; }
        public CodeRushStateUpdate()
        {

        }

        public StreamDeckData GetStreamDeckData()
        {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(CodeRushStateUpdate) };
        }
    }
}
