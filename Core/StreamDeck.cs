using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using CodeRushStreamDeck.Startup;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using PipeCore;
using Pipes.Server;
using StreamDeckLib;
using System.Collections.Specialized;

namespace CodeRushStreamDeck
{

    public static class StreamDeck
    {
        static StreamDeck()
        {
            ConnectionManager.DeviceDidConnect += ConnectionManager_DeviceDidConnect;
            ConnectionManager.DeviceDidDisconnect += ConnectionManager_DeviceDidDisconnect;
        }

        private static void ConnectionManager_DeviceDidDisconnect(object sender, StreamDeckLib.Messages.StreamDeckEventPayload e)
        {
            StartupInfo.RemoveDeviceById(e.device);
        }

        private static void ConnectionManager_DeviceDidConnect(object sender, StreamDeckLib.Messages.StreamDeckEventPayload e)
        {
            StartupInfo.AddDevice(e.device, e.deviceInfo.type, e.deviceInfo.size);
        }

        public static void SetConnectionManager(ConnectionManager connectionManager)
        {
            Manager = connectionManager;
        }

        public static async void SwitchToProfile(string profileName, string device)
        {
            string uuid = Manager.GetInstanceUuid();
            await Manager.SwitchToProfileAsync(uuid, device, profileName);
        }

        static void SendDeviceInfoToCodeRush()
        {
            DeviceInformation deviceInformation = CommandHelper.GetDeviceInformation();
            foreach (Device device in StartupInfo.Devices)
            {
                deviceInformation.Devices.Add(new StreamDeckDevice()
                {
                    Id = device.id,
                    Type = device.type,
                    Size = new StreamDeckSize()
                    {
                        Columns = device.size.columns,
                        Rows = device.size.rows,
                    },
                    Name = device.name
                });
            }

            string data = JsonConvert.SerializeObject(deviceInformation);
            CommunicationServer.SendMessageToCodeRush(data, nameof(DeviceInformation));
        }

        public static void HandleCommandFromCodeRush(string command)
        {
            if (command == CommandsFromCodeRush.GetDeviceInfo)
                SendDeviceInfoToCodeRush();
        }

        public static ConnectionManager Manager { get; private set; }
    }
}
