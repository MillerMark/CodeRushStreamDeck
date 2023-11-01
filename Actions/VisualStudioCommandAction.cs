using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Models;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using Pipes.Server;
using PipeCore;
using CodeRushStreamDeck.Startup;
using System.Diagnostics;
using System.Runtime.Versioning;
using static CodeRushStreamDeck.VisualStudioCommandAction;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devex.cr.exec.vs.command")]
    public class VisualStudioCommandAction : StreamDeckButton<VisualStudioCommandModel>, IStreamDeckButton
    {
        CarouselHelper carouselHelper = new();

        public override async Task OnPropertyInspectorDidAppear(StreamDeckEventPayload args)
        {
            await base.OnPropertyInspectorDidAppear(args);
            VisualStudio.GetDataIfNeeded();
            if (VisualStudio.IsInitialized)
                await SendVisualStudioCommandsToPropertyInspector();
            else
                VisualStudio.CommandsInitialized += VisualStudio_CommandsInitialized;
        }

        public override Task OnPropertyInspectorDidDisappear(StreamDeckEventPayload args)
        {
            VisualStudio.CommandsInitialized -= VisualStudio_CommandsInitialized;
            return base.OnPropertyInspectorDidDisappear(args);
        }

        private async void VisualStudio_CommandsInitialized(object sender, EventArgs e)
        {
            VisualStudio.CommandsInitialized -= VisualStudio_CommandsInitialized;
            await SendVisualStudioCommandsToPropertyInspector();
        }

        async Task SendVisualStudioCommandsToPropertyInspector()
        {
            await CommandLoader.LoadVisualStudioCommands(Manager, lastContext);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await carouselHelper.ShowImage(Manager, args, SettingsModel);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            await carouselHelper.OnDidReceiveSettings(Manager, args, SettingsModel);
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            if (!string.IsNullOrEmpty(SettingsModel.Command))
                SendVisualStudioCommandToCodeRush(SettingsModel.Command, SettingsModel.Parameters, ButtonState.Down);

            // TODO: Remove this test code.
            if (SettingsModel.Command == "Debug.Breakpoints")
                await Manager.GetSettingsAsync(args.context);
        }

        void SendVisualStudioCommandToCodeRush(string command, string parameters, ButtonState buttonState)
        {
            CommunicationServer.SendMessageToCodeRush(CommandHelper.GetVisualStudioCommandData(command, parameters, buttonState, buttonInstanceId));
        }
    }
}
