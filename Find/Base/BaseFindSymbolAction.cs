﻿using StreamDeckLib;
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
    public abstract class BaseFindSymbolAction : BaseStreamDeckAction // BaseStreamDeckActionWithSettingsModel<Models.CounterSettingsModel>
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

        VoiceCommandData GetVoiceCommandData()
        {
            VoiceCommandData voiceCommandData = new VoiceCommandData();
            InitializeCommandData(voiceCommandData);
            voiceCommandData.SpokenWordsStart = SpokenWordsStart;
            voiceCommandData.SpokenWordsEnd = SpokenWordsEnd;
            voiceCommandData.ButtonState = ButtonState.Down;
            return voiceCommandData;
        }

        private void InitializeCommandData(ButtonStreamDeckData buttonData)
        {
            buttonData.StreamDeckPluginVersion_Major = Version.Major;
            buttonData.StreamDeckPluginVersion_Minor = Version.Minor;
            buttonData.ButtonId = id;
        }

        CommandData GetCommandData(string command, ButtonState buttonState)
        {
            CommandData commandData = new CommandData() { Command = command };
            commandData.ButtonState = buttonState;
            InitializeCommandData(commandData);
            return commandData;
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            lastContext = args.context;
            keysDown.TryAdd(id, this);
            string data = JsonConvert.SerializeObject(GetVoiceCommandData());
            CommunicationServer.SendMessageToCodeRush(data, nameof(VoiceCommandData));
        }

        void SendCommandToCodeRush(string command, ButtonState buttonState)
        {
            string data = JsonConvert.SerializeObject(GetCommandData(command, buttonState));
            CommunicationServer.SendMessageToCodeRush(data, nameof(CommandData));
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            //SettingsModel.Counter++;
            
            //await Manager.ShowOkAsync(args.context);
            //await Manager.ShowAlertAsync(args.context);
            //await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());

            await Manager.SetImageAsync(args.context, $"images/symbols/{SymbolName}.png");

            //update settings
            //await Manager.SetSettingsAsync(args.context, SettingsModel);

            keysDown.Remove(id);
            
            SendCommandToCodeRush(StreamDeckCommands.StopListening, ButtonState.Up);
            await ClearTitle(args);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            //await Manager.SetTitleAsync(args.context, SettingsModel.Counter.ToString());
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await ClearTitle(args);
        }

        private async Task ClearTitle(StreamDeckEventPayload args)
        {
            await Manager.SetTitleAsync(args.context, string.Empty);
        }

        public static void ListeningStarted(string buttonId)
        {
            if (keysDown.TryGetValue(buttonId, out BaseFindSymbolAction baseFindSymbolAction))
                baseFindSymbolAction.ListeningStartedAsync();
        }

        async void ListeningStartedAsync()
        {
            await Manager.SetImageAsync(lastContext, $"images/symbols/{SymbolName}Listening.png");
        }

        async void UpdateVolume(int volume)
        {
            int inBoundsVolume = Math.Min(7, Math.Max(0, volume));
            string newTitle = "  " + new string('I', inBoundsVolume) + new string(' ', 7 - inBoundsVolume);
            Debug.WriteLine($"Level: \"{newTitle}\"");
            await Manager.SetTitleAsync(lastContext, newTitle);
        }

        public static void UpdateVolume(string buttonId, int volume)
        {
            if (keysDown.TryGetValue(buttonId, out BaseFindSymbolAction baseFindSymbolAction))
                baseFindSymbolAction.UpdateVolume(volume);
        }
    }
}
