using System.Collections.Generic;
using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class AvailableCommands : FromCodeRushData
    {
        public List<string> VisualStudioCommands { get; set; }
        public List<string> CodeRushCommands { get; set; }

        public AvailableCommands()
        {

        }

        public StreamDeckData GetStreamDeckData()
        {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(AvailableCommands) };
        }
    }
}
