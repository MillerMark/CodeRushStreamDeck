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

        static void HandleDataReceived(StreamDeckData streamDeckData)
        {
            switch (streamDeckData.DataType)
            {
                case nameof(ShowListeningOnStreamDeck):
                    var showListeningOnStreamDeck = JsonConvert.DeserializeObject<ShowListeningOnStreamDeck>(streamDeckData.Data);
                    BaseFindSymbolAction.ListeningStarted(showListeningOnStreamDeck.ButtonID);
                    break;
                case nameof(ShowVolumeOnStreamDeck):
                    var showVolumeOnStreamDeck = JsonConvert.DeserializeObject<ShowVolumeOnStreamDeck>(streamDeckData.Data);
                    BaseFindSymbolAction.UpdateVolume(showVolumeOnStreamDeck.ButtonID, showVolumeOnStreamDeck.Volume);
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


