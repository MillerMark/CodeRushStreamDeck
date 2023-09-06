using Newtonsoft.Json;
using System;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class ShowListeningOnStreamDeck : FromCodeRushData
    {
        public string ButtonID { get; set; }
        public ShowListeningOnStreamDeck()
        {
        }

        public StreamDeckData GetStreamDeckData() {
            var serializeObject = JsonConvert.SerializeObject(this);
            return new StreamDeckData() { Data = serializeObject, DataType = nameof(ShowListeningOnStreamDeck) };
        }
    }
}
