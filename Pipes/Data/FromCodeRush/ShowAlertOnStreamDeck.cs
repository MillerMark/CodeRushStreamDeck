using Newtonsoft.Json;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class ShowAlertOnStreamDeck : FromCodeRushData
    {
        public string ButtonID { get; set; }
        public ShowAlertOnStreamDeck()
        {

        }

        public StreamDeckData GetStreamDeckData() {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(ShowAlertOnStreamDeck) };
        }
    }
}
