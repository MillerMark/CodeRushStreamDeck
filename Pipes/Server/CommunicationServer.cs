using CodeRushStreamDeck;
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

        public static void SendMessageToCodeRush(string message)
        {
            if (messageSender == null)
                Start();
            messageSender.EnqueueMessage(message);
        }

        private static void MessageReceiver_MessageReceived(object sender, string e)
        {
            BaseFindSymbolAction.ListeningStarted(e);
        }
    }
}


