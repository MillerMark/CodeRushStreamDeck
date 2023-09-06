using System;
using System.Linq;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class VoiceCommandData : ButtonStreamDeckData
    {
        public string SpokenWordsStart { get; set; }
        public string SpokenWordsEnd { get; set; }
        public VoiceCommandData()
        {
        }
    }
}
