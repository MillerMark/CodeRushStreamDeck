using Newtonsoft.Json;
using StreamDeckLib;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace CodeRushStreamDeck.Startup
{
    public class StartupInfo
    {
        public Application application { get; set; }
        public Colors colors { get; set; }
        public int devicePixelRatio { get; set; }
        public List<Device> devices { get; set; }
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

        public static Application Application => startupInfo.application;
        public static Colors Colors => startupInfo.colors;
        public static int DevicePixelRatio => startupInfo.devicePixelRatio;
        public static List<Device> Devices => startupInfo.devices;
        public static Plugin Plugin => startupInfo.plugin;
    }
}
