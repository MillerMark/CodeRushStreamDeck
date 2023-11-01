using Pipes.Server;
using System;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using PipeCore;
using Microsoft.Extensions.Logging;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Models;

namespace CodeRushStreamDeck
{
    /// <summary>
    /// This is for CodeRush commands that toggle a particular state.
    /// </summary>
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.command")]
    public class CodeRushCommandAction : CustomDrawButton<CodeRushCommandModel>
    {
        bool enabled;
        ScrollingText scrollingText;
        CarouselHelper carouselHelper = new();

        // TODO: Change the BackgroundImageName
        protected override string BackgroundImageName => "CodeRushTemplate";

        protected override void RefreshButtonImage(Graphics background)
        {
            base.RefreshButtonImage(background);

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

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            await carouselHelper.OnDidReceiveSettings(Manager, args, SettingsModel);
            scrollingText?.InvalidateDrawingParameters();
            await UpdateImageAsync();
        }

        protected override Bitmap GetIconImage()
        {
            return carouselHelper.GetImage(SettingsModel);
        }

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
