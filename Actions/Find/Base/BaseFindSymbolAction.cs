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
using PipeCore;

namespace CodeRushStreamDeck
{
    public abstract class BaseFindSymbolAction : BaseStreamDeckAction, IStreamDeckButton
    {
        public abstract string SymbolName { get; }
        public virtual string SpokenWordsStart { get; } = string.Empty;
        public virtual string SpokenWordsEnd { get; } = string.Empty;
        public string Id => buttonInstanceId;


        protected string buttonInstanceId = Guid.NewGuid().ToString();
        string lastContext;
        public BaseFindSymbolAction()
        {
        }

        VoiceCommandData GetVoiceCommandData()
        {
            VoiceCommandData voiceCommandData = new VoiceCommandData();
            CommandHelper.InitializeButtonData(voiceCommandData, buttonInstanceId);
            voiceCommandData.SpokenWordsStart = SpokenWordsStart;
            voiceCommandData.SpokenWordsEnd = SpokenWordsEnd;
            voiceCommandData.ButtonState = ButtonState.Down;
            return voiceCommandData;
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            lastContext = args.context;
            ButtonTracker.OnKeyDown(this);
            CommunicationServer.SendMessageToCodeRush(GetVoiceCommandData());
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            await base.OnKeyUp(args);
            await Manager.SetImageAsync(args.context, $"images/symbols/{SymbolName}.png");
            ButtonTracker.OnKeyUp(this);
            SendCommandToCodeRush(CommandsFromStreamDeck.StopListening, ButtonState.Up);
            await ClearTitle(args);
        }

        void SendCommandToCodeRush(string command, ButtonState buttonState)
        {
            CommunicationServer.SendMessageToCodeRush(CommandHelper.GetCommandData(command, buttonState, buttonInstanceId));
        }

        public override Task OnWillAppear(StreamDeckEventPayload args)
        {
            base.OnWillAppear(args).FireAndForget();
            ClearTitle(args).FireAndForget();
            return Task.CompletedTask;
        }

        private async Task ClearTitle(StreamDeckEventPayload args)
        {
            await Manager.SetTitleAsync(args.context, string.Empty);
        }

        public static void ListeningStarted(string buttonId)
        {
            BaseFindSymbolAction baseFindSymbolAction = GetFindSymbolAction(buttonId);
            if (baseFindSymbolAction != null)
                baseFindSymbolAction.ListeningStartedAsync();
        }

        private static BaseFindSymbolAction GetFindSymbolAction(string buttonId)
        {
            return ButtonTracker.Get(buttonId) as BaseFindSymbolAction;
        }

        async void ListeningStartedAsync()
        {
            await Manager.SetImageAsync(lastContext, $"images/symbols/{SymbolName}Listening.png");
        }

        async void UpdateVolume(int volume)
        {
            int streamDeckVolume = (int)(8 * (volume - 1) / 100);  // Gets a volume from 0-7.
            int inBoundsVolume = Math.Min(7, Math.Max(0, streamDeckVolume));
            string newTitle = "  " + new string('I', inBoundsVolume) + new string(' ', 7 - inBoundsVolume);
            Debug.WriteLine($"Level: \"{newTitle}\"");
            await Manager.SetTitleAsync(lastContext, newTitle);
        }

        public static void UpdateVolume(string buttonId, int volume)
        {
            BaseFindSymbolAction baseFindSymbolAction = GetFindSymbolAction(buttonId);
            if (baseFindSymbolAction != null)
                baseFindSymbolAction.UpdateVolume(volume);
        }

        public override Task OnDeviceDidConnect(StreamDeckEventPayload args)
        {
            string deviceId = args.device;

            //args.deviceInfo;
            //args.deviceInfo.type;
            //args.deviceInfo.size;

            return base.OnDeviceDidConnect(args);
        }

        public override Task OnDeviceDidDisconnect(StreamDeckEventPayload args)
        {
            return base.OnDeviceDidDisconnect(args);
        }

        public async void ShowAlert()
        {
            await Manager.ShowAlertAsync(lastContext);
        }
    }
}
