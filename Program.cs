using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Settings.Configuration;
using StreamDeckLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Pipes.Server;
using CodeRushStreamDeck.Startup;
using Newtonsoft.Json;

namespace CodeRushStreamDeck
{

    class Program
    {
        static async Task Main(string[] args)
        {

            StartupInfo.Initialize(args);
            CommunicationServer.Start();
            using (var config = StreamDeckLib.Config.ConfigurationBuilder.BuildDefaultConfiguration(args))
            {
                var connectionManager = ConnectionManager.Initialize(args, config.LoggerFactory);
                StreamDeck.SetConnectionManager(connectionManager);
                await connectionManager.RegisterAllActions(typeof(Program).Assembly).StartAsync();
                StreamDeck.Initialize();
            }
        }

    }
}
