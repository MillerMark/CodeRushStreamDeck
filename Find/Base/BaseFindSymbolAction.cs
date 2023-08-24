using StreamDeckLib;
using StreamDeckLib.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Pipes.Server;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;

namespace CodeRushStreamDeck
{
    public abstract class BaseFindSymbolAction : BaseStreamDeckActionWithSettingsModel<Models.CounterSettingsModel>
    {
        public abstract string SymbolName { get; }
        public virtual string SpokenWordsStart { get; } = string.Empty;
        public virtual string SpokenWordsEnd { get; } = string.Empty;

        static Dictionary<string, BaseFindSymbolAction> keysDown = new();
        protected string id = Guid.NewGuid().ToString();
        string lastContext;
        public BaseFindSymbolAction()
        {
        }

        public static void ListeningStarted(string buttonId)
        {
            if (keysDown.TryGetValue(buttonId, out BaseFindSymbolAction baseFindSymbolAction))
            {
                baseFindSymbolAction.ListeningStartedAsync();
            }
        }

        async void ListeningStartedAsync()
        {
            await Manager.SetImageAsync(lastContext, $"images/symbols/{SymbolName}Listening.png");
        }

        VoiceCommandData GetVoiceCommandData()
        {
            VoiceCommandData voiceCommandData = new VoiceCommandData();
            voiceCommandData.StreamDeckPluginVersion_Major = Version.Major;
            voiceCommandData.StreamDeckPluginVersion_Minor = Version.Minor;
            voiceCommandData.SpokenWordsStart = SpokenWordsStart;
            voiceCommandData.SpokenWordsEnd = SpokenWordsEnd;
            voiceCommandData.ButtonId = id;
            voiceCommandData.ButtonState = ButtonState.Down;
            return voiceCommandData;
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            VoiceCommandData buttonStreamDeckData = GetVoiceCommandData();

            lastContext = args.context;
            keysDown.Add(id, this);
            string serializedObject = JsonConvert.SerializeObject(buttonStreamDeckData);
            CommunicationServer.SendMessageToCodeRush(serializedObject, nameof(VoiceCommandData));
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            SettingsModel.Counter++;
            //await Manager.ShowOkAsync(args.context);
            //await Manager.ShowAlertAsync(args.context);
            //await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());

            await Manager.SetImageAsync(args.context, $"images/symbols/{SymbolName}.png");

            //update settings
            await Manager.SetSettingsAsync(args.context, SettingsModel);
            keysDown.Remove(id);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());
        }

    }
}
