using System;
using System.Runtime.Versioning;
using PipeCore;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using Pipes.Server;
using StreamDeckLib.Messages;
using System.Threading.Tasks;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public abstract class VoiceButton<T> : CustomDrawButton<T>
    {
        VoiceCommandData GetVoiceCommandData()
        {
            VoiceCommandData voiceCommandData = new VoiceCommandData();
            CommandHelper.InitializeButtonData(voiceCommandData, buttonInstanceId);
            voiceCommandData.SpokenWordsStart = "Method";
            voiceCommandData.SpokenWordsEnd = "";
            voiceCommandData.ButtonState = ButtonState.Down;
            return voiceCommandData;
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            string data = JsonConvert.SerializeObject(GetVoiceCommandData());
            CommunicationServer.SendMessageToCodeRush(data, nameof(VoiceCommandData));
        }
    }
}
