using Newtonsoft.Json;
using StreamDeckLib;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using static StreamDeckLib.Messages.Info;

namespace CodeRushStreamDeck.Startup
{
    public class StartupInfo
    {
        public Application application { get; set; }
        public Colors colors { get; set; }
        public int devicePixelRatio { get; set; }
        public List<Device> devices { get; set; } = new();
        static List<Device> removedDevices { get; set; } = new();
        public Plugin plugin { get; set; }

        static StartupInfo startupInfo;

        public static void Initialize(string[] args)
        {
            bool nextArgContainsInfo = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (nextArgContainsInfo)
                {
                    startupInfo = JsonConvert.DeserializeObject<StartupInfo>(args[i]);
                    break;
                }
                if (args[i] == "-info")
                    nextArgContainsInfo = true;
            }
        }

        public static void RemoveDeviceById(string deviceId)
        {
            if (Devices == null)
                return;

            Device deviceToRemove = GetDeviceById(deviceId);
            if (deviceToRemove != null)
            {
                Device existingRemovedDevice = GetRemovedDeviceById(deviceId);
                if (existingRemovedDevice == null)
                    removedDevices.Add(deviceToRemove);  // Track it in case we plug it back in so we can keep the name.

                Devices.Remove(deviceToRemove);
            }
        }

        private static Device GetDeviceById(string deviceId)
        {
            return Devices.FirstOrDefault(x => x.id == deviceId);
        }

        private static Device GetRemovedDeviceById(string deviceId)
        {
            return removedDevices.FirstOrDefault(x => x.id == deviceId);
        }

        public static void AddDevice(string device, int type, StreamDeckLib.Messages.Size size)
        {
            if (Devices == null)
                startupInfo.devices = new();

            Device existingDevice = GetDeviceById(device);
            if (existingDevice != null)
                return;

            Device removedDeviceById = GetRemovedDeviceById(device);

            string name;

            if (removedDeviceById == null)
                name = "Unknown device name (recently plugged in?)";
            else
                name = removedDeviceById.name;

            Devices.Add(new Device()
            {
                id = device,
                name = name,
                type = type,
                size = new Size()
                {
                    columns = size.columns,
                    rows = size.rows
                }
            });
        }

        public static Application Application => startupInfo.application;
        public static Colors Colors => startupInfo.colors;
        public static int DevicePixelRatio => startupInfo.devicePixelRatio;
        public static List<Device> Devices => startupInfo.devices;
        public static Plugin Plugin => startupInfo.plugin;
    }
}
