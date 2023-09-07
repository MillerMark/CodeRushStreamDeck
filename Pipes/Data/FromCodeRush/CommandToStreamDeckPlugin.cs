using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class CommandToStreamDeckPlugin : FromCodeRushData
    {
        public string Command { get; set; }

        public CommandToStreamDeckPlugin()
        {

        }

        public StreamDeckData GetStreamDeckData()
        {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(CommandToStreamDeckPlugin) };
        }
    }
}
