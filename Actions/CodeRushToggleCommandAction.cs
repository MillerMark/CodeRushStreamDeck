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
using System.Timers;
using Microsoft.Extensions.Logging;

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
        ScrollingText scrollingText;
        CarouselHelper carouselHelper = new();

        protected override string BackgroundImageName => "CodeRushTemplate";

        protected override void RefreshButtonImage(Graphics background)
        {
            base.RefreshButtonImage(background);
            if (!enabled)
            {
                // Dim down existing button
                Brush brush = new SolidBrush(Color.FromArgb(80, Color.Black));
                background.FillRectangle(brush, 0, 0, 144, 144);
            }

            if (scrollingText == null)
                scrollingText = new ScrollingText(background, SettingsModel, ButtonText_RefreshImage);
            else
                scrollingText.CheckDrawingParameters(background, SettingsModel);

            scrollingText.Draw(background);
        }

        private async void ButtonText_RefreshImage(object sender, EventArgs e)
        {
            try
            {
                await UpdateImageAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            enabled = Variables.GetBool(SettingsModel.StateName);
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            await carouselHelper.OnDidReceiveSettings(Manager, args, SettingsModel);
            scrollingText?.InvalidateDrawingParameters();
            await UpdateImageAsync();
        }

        public CodeRushToggleCommandAction()
        {
            Variables.BoolVarChanged += Variables_BoolVarChanged;
        }

        protected override Bitmap GetIconImage()
        {
            return carouselHelper.GetImage(SettingsModel);
        }

        protected override void DrawBackgroundBehindImage(Graphics graphics)
        {
            if (enabled)
                DrawVisualStudioRectangle(graphics);
        }
        
        private async void Variables_BoolVarChanged(object sender, VarEventArgs<bool> e)
        {
            if (e.Name == SettingsModel.StateName && enabled != e.Value)
            {
                enabled = e.Value;
                if (enabled)
                {
                    IconTop = 6;
                    IconLeft = 12;
                    IconScale = 0.8f;
                }
                else
                {
                    IconTop = 0;
                    IconLeft = 0;
                    IconScale = 1f;
                }
                await UpdateImageAsync();
            }
        }

        private static void DrawVisualStudioRectangle(Graphics background)
        {
            const float topMargin = 4;
            const float horizontalMargin = 6;
            const float thickness = 6;
            const float textHeight = 42f;
            const float buttonSize = 144f;
            Pen pen = new Pen(Color.FromArgb(113, 96, 232), thickness);
            background.DrawRectangle(pen, horizontalMargin, topMargin, buttonSize - horizontalMargin * 2, buttonSize - topMargin - textHeight);
        }

        // TODO: Need an image picker just like we have for the VS commands.
        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            CommunicationServer.SendMessageToCodeRush(
                    CommandHelper.GetCodeRushCommandData(
                        SettingsModel.Command,
                        SettingsModel.Parameters,
                        SettingsModel.Context,
                        ButtonState.Up,
                        buttonInstanceId));
        }
    }
}
