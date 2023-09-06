using System;
using System.Linq;
using CodeRushStreamDeck.Startup;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    public static class StreamDeck
    {
        public static void SetConnectionManager(ConnectionManager connectionManager)
        {
            Manager = connectionManager;
        }

        public static async void SwitchToProfile(string profileName, string device)
        {
            string uuid = Manager.GetInstanceUuid();
            await Manager.SwitchToProfileAsync(uuid, device, profileName);
        }

        public static ConnectionManager Manager { get; private set; }
    }
}
