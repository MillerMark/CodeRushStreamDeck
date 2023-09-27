using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Timers;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.spoken.type.template")]
    public class SpokenTypeTemplate : VoiceButton<Models.TypeContentCreationButton>
    {
        protected override string BackgroundImageName => "AddMethod";

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            // TODO: Send additional data to expand the template once the type is received.
            SendCommandToCodeRush(CommandsFromStreamDeck.GetSpokenType, ButtonState.Down);
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
