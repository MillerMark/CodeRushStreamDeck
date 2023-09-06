using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class SwitchToProfileOnStreamDeck : FromCodeRushData
    {
        public string ProfileName { get; set; }
        public string DeviceId { get; set; }
            
        public SwitchToProfileOnStreamDeck()
        {

        }

        public StreamDeckData GetStreamDeckData()
        {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(SwitchToProfileOnStreamDeck) };
        }
    }
}
