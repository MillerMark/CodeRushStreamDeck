using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Timers;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using PipeCore;
using Pipes.Server;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.spoken.type.template")]
    public class SpokenTypeTemplate : VoiceButton<Models.SpokenTypeTemplateData>
    {
        protected override string BackgroundImageName => "AddMethod";

        void SendRequestForSpokenTypeToCodeRush(ButtonState buttonState)
        {
            CommunicationServer.SendMessageToCodeRush(
                CommandHelper.GetSpokenTypeData(
                    SettingsModel.TemplateToExpand, 
                    SettingsModel.Context, 
                    buttonState, 
                    buttonInstanceId));
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            // TODO: Send additional data to expand the template once the type is received.
            SendRequestForSpokenTypeToCodeRush(ButtonState.Down);
        }

        public override async void TypeRecognized(TypeRecognizedFromSpokenWords typeRecognizedFromSpokenWords)
        {
            SettingsModel.GenericType = typeRecognizedFromSpokenWords.GenericType;
            SettingsModel.SimpleType = typeRecognizedFromSpokenWords.SimpleType;
            SettingsModel.Kind = typeRecognizedFromSpokenWords.Kind;
            SettingsModel.TypeParam1 = typeRecognizedFromSpokenWords.TypeParam1;
            SettingsModel.TypeParam2 = typeRecognizedFromSpokenWords.TypeParam2;
            await Manager.SetSettingsAsync(lastContext, SettingsModel);
        }
    }
}
