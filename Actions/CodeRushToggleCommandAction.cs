using Pipes.Server;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using System.Collections.Generic;
using PipeCore;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Models;
using System.Drawing;

namespace CodeRushStreamDeck
{
    /// <summary>
    /// This is for CodeRush commands that toggle a particular state.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.toggle.command")]
    public class CodeRushToggleCommandAction : CustomDrawButton<CodeRushToggleStateCommandModel>
    {
        bool enabled;
        protected override string BackgroundImageName => "CodeRushTemplate";

        public CodeRushToggleCommandAction()
        {
            Variables.BoolVarChanged += Variables_BoolVarChanged; ;
        }

        private async void Variables_BoolVarChanged(object sender, VarEventArgs<bool> e)
        {
            if (e.Name == SettingsModel.StateName && enabled != e.Value)
            {
                enabled = e.Value;
                await UpdateImageAsync();
            }
        }

        protected override void RefreshButtonImage(Graphics background)
        {
            base.RefreshButtonImage(background);
            if (enabled)
            {
                const float margin = 2;
                const float thickness = 2;
                const float textHeight = 50f;
                const float buttonSize = 144f;
                Pen pen = new Pen(Color.FromArgb(113, 96, 232), thickness);
                background.DrawRectangle(pen, margin, margin, buttonSize - margin * 2, buttonSize - margin - textHeight);
            }
        }

        // TODO: Need an image picker just like we have for the VS commands.
        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            // TODO: Implement this!
            //CommunicationServer.SendCodeRushCommand();
        }
    }
}
