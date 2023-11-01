using CodeRushStreamDeck;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using PipeCore;
using System;


namespace Pipes.Server
{
    public static class CommunicationServer
    {
        static MessageReceiverServer messageReceiver;
        static MessageSenderServer messageSender;

        public static void Start()
        {
            if (messageReceiver != null)
                return;

            messageReceiver = new MessageReceiverServer(Constants.CodeRushToStreamDeck);
            messageReceiver.Start();
            messageReceiver.MessageReceived += MessageReceiver_MessageReceived;


            messageSender = new MessageSenderServer(Constants.StreamDeckToCodeRush);
            messageSender.Start();
        }

        public static void SendMessageToCodeRush(string data, string dataType)
        {
            if (messageSender == null)
                Start();

            StreamDeckData streamDeckData = new StreamDeckData() { Data = data, DataType = dataType };

            messageSender.EnqueueMessage(JsonConvert.SerializeObject(streamDeckData));

            RequestCodeRushDataIfNeeded();
        }

        public static void SendMessageToCodeRush(object message)
        {
            string data = JsonConvert.SerializeObject(message);
            SendMessageToCodeRush(data, message.GetType().Name);
        }

        public static void VoiceButtonListeningStarted(string buttonId)
        {
            IVoiceButton voiceButton = ButtonTracker.GetDown(buttonId) as IVoiceButton;
            if (voiceButton != null)
                voiceButton.ListeningStarted();
        }

        static void UpdateVolume(string buttonId, int volume)
        {
            IVoiceButton voiceButton = ButtonTracker.GetDown(buttonId) as IVoiceButton;
            if (voiceButton != null)
                voiceButton.UpdateVolume(volume);
        }

        static void TypeRecognized(TypeRecognizedFromSpokenWords typeRecognizedFromSpokenWords)
        {
            IVoiceButton voiceButton = ButtonTracker.Get(typeRecognizedFromSpokenWords.ButtonID) as IVoiceButton;
            if (voiceButton != null)
                voiceButton.TypeRecognized(typeRecognizedFromSpokenWords);
        }

        static void UpdateBooleanStates(CodeRushStateUpdate stateUpdate)
        {
            foreach (string key in stateUpdate.State.Keys)
                Variables.SetBool(key, stateUpdate.State[key]);
        }

        static void HandleDataReceived(StreamDeckData streamDeckData)
        {
            switch (streamDeckData.DataType)
            {
                case nameof(CodeRushStateUpdate):
                    var stateUpdate = JsonConvert.DeserializeObject<CodeRushStateUpdate>(streamDeckData.Data);
                    UpdateBooleanStates(stateUpdate);
                    break;

                case nameof(ShowListeningOnStreamDeck):
                    var showListeningOnStreamDeck = JsonConvert.DeserializeObject<ShowListeningOnStreamDeck>(streamDeckData.Data);
                    
                    
                    // TODO: Delete this call: BaseFindSymbolAction.ListeningStarted after converting Find buttons to VoiceButtons...
                    BaseFindSymbolAction.ListeningStarted(showListeningOnStreamDeck.ButtonID);
                    
                    
                    VoiceButtonListeningStarted(showListeningOnStreamDeck.ButtonID);
                    break;
                case nameof(ShowVolumeOnStreamDeck):
                    var showVolumeOnStreamDeck = JsonConvert.DeserializeObject<ShowVolumeOnStreamDeck>(streamDeckData.Data);


                    // TODO: Delete this call: BaseFindSymbolAction.UpdateVolume after converting Find buttons to VoiceButtons...
                    BaseFindSymbolAction.UpdateVolume(showVolumeOnStreamDeck.ButtonID, showVolumeOnStreamDeck.Volume);


                    UpdateVolume(showVolumeOnStreamDeck.ButtonID, showVolumeOnStreamDeck.Volume);
                    break;
                case nameof(TypeRecognizedFromSpokenWords):
                    var typeRecognizedFromSpokenWords = JsonConvert.DeserializeObject<TypeRecognizedFromSpokenWords>(streamDeckData.Data);

                    TypeRecognized(typeRecognizedFromSpokenWords);
                    break;
                case nameof(CommandToStreamDeckPlugin):
                    var commandToStreamDeck = JsonConvert.DeserializeObject<CommandToStreamDeckPlugin>(streamDeckData.Data);
                    StreamDeck.HandleCommandFromCodeRush(commandToStreamDeck.Command);
                    break;
                case nameof(ShowAlertOnStreamDeck):
                    var showAlertOnStreamDeck = JsonConvert.DeserializeObject<ShowAlertOnStreamDeck>(streamDeckData.Data);
                    ButtonTracker.ShowAlert(showAlertOnStreamDeck.ButtonID);
                    break;
                case nameof(SwitchToProfileOnStreamDeck):
                    var switchToProfileOnStreamDeck = JsonConvert.DeserializeObject<SwitchToProfileOnStreamDeck>(streamDeckData.Data);
                    StreamDeck.SwitchToProfile(switchToProfileOnStreamDeck.ProfileName, switchToProfileOnStreamDeck.DeviceId);
                    break;
            }
            //ShowListeningOnStreamDeck
            //FromCodeRushData
            
        }

        private static void MessageReceiver_MessageReceived(object sender, string e)
        {
            StreamDeckData streamDeckData = JsonConvert.DeserializeObject<StreamDeckData>(e);
            if (streamDeckData == null)
                return;
            HandleDataReceived(streamDeckData);
            RequestCodeRushDataIfNeeded();
        }

        private static void RequestCodeRushDataIfNeeded()
        {
            if (!StreamDeck.CommandsExist)
                StreamDeck.RequestCommands();
        }

        public static void SendSimpleCommandToCodeRush(string simpleCommand, ButtonState buttonState = ButtonState.None, string buttonId = null)
        {
            if (buttonId == null)
                buttonId = Guid.Empty.ToString();
            SendMessageToCodeRush(CommandHelper.GetCommandData(simpleCommand, buttonState, buttonId));
        }
    }
}


