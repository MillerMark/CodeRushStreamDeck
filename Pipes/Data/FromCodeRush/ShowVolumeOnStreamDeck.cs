using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class ShowVolumeOnStreamDeck : FromCodeRushData
    {
        public string ButtonID { get; set; }
        public int Volume { get; set; }
        public ShowVolumeOnStreamDeck()
        {
        }

        public StreamDeckData GetStreamDeckData()
        {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(ShowVolumeOnStreamDeck) };
        }
    }
}
