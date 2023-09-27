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
        }

        public static void SendMessageToCodeRush(object message)
        {
            string data = JsonConvert.SerializeObject(message);
            SendMessageToCodeRush(data, message.GetType().Name);
        }

        public static void VoiceButtonListeningStarted(string buttonId)
        {
            IVoiceButton voiceButton = ButtonTracker.Get(buttonId) as IVoiceButton;
            if (voiceButton != null)
                voiceButton.ListeningStarted();
        }

        static void UpdateVolume(string buttonId, int volume)
        {
            IVoiceButton voiceButton = ButtonTracker.Get(buttonId) as IVoiceButton;
            if (voiceButton != null)
                voiceButton.UpdateVolume(volume);
        }

        static void HandleDataReceived(StreamDeckData streamDeckData)
        {
            switch (streamDeckData.DataType)
            {
                case nameof(ShowListeningOnStreamDeck):
                    var showListeningOnStreamDeck = JsonConvert.DeserializeObject<ShowListeningOnStreamDeck>(streamDeckData.Data);
                    
                    
                    // TODO: Delete this call: BaseFindSymbolAction.ListeningStarted...
                    BaseFindSymbolAction.ListeningStarted(showListeningOnStreamDeck.ButtonID);
                    
                    
                    VoiceButtonListeningStarted(showListeningOnStreamDeck.ButtonID);
                    break;
                case nameof(ShowVolumeOnStreamDeck):
                    var showVolumeOnStreamDeck = JsonConvert.DeserializeObject<ShowVolumeOnStreamDeck>(streamDeckData.Data);


                    // TODO: Delete this call: BaseFindSymbolAction.UpdateVolume...
                    BaseFindSymbolAction.UpdateVolume(showVolumeOnStreamDeck.ButtonID, showVolumeOnStreamDeck.Volume);


                    UpdateVolume(showVolumeOnStreamDeck.ButtonID, showVolumeOnStreamDeck.Volume);
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
        }
    }
}


