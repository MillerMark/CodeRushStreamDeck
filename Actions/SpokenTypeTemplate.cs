using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;
using System.Collections.Generic;
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
        const string STR_FontName = "Arial";
        Timer marqueeTimer = new Timer(50);

        public SpokenTypeTemplate()
        {
            marqueeTimer.Elapsed += MarqueeTimer_Elapsed;
        }

        private void MarqueeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

        }

        protected override string BackgroundImageName => "AddMethod";

        void SendRequestForSpokenTypeToCodeRush(ButtonState buttonState)
        {
            GetVariablesToSetAndTemplateToExpand(out string variablesToSet, out string templateToExpand);
            CommunicationServer.SendMessageToCodeRush(
                CommandHelper.GetSpokenTypeData(
                    templateToExpand,
                    SettingsModel.Context,
                    variablesToSet,
                    buttonState,
                    buttonInstanceId));
        }

        private void GetVariablesToSetAndTemplateToExpand(out string variablesToSet, out string templateToExpand)
        {
            Variables.ClearDynamicListEntries();
            variablesToSet = Variables.Expand("$isStatic$\n$scope$");
            templateToExpand = Variables.Expand(SettingsModel.TemplateToExpand);
        }
        
        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            SendRequestForSpokenTypeToCodeRush(ButtonState.Down);
        }

        TypeInformation GetTypeInformation()
        {
            return new TypeInformation() { 
                Kind = SettingsModel.Kind,
                SimpleType = SettingsModel.SimpleType,
                GenericType = SettingsModel.GenericType,
                TypeParam1 = SettingsModel.TypeParam1,
                TypeParam2 = SettingsModel.TypeParam2,
            }; 
        }
        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            await base.OnKeyUp(args);
            if (!heardAudio)
            {
                GetVariablesToSetAndTemplateToExpand(out string variablesToSet, out string templateToExpand);
                List<DynamicListEntry> dynamicListEntries = Variables.DynamicListEntries;
                string template = dynamicListEntries.GetTemplateToExpand(SettingsModel.Kind, templateToExpand, GetTypeInformation());
                
                CommunicationServer.SendMessageToCodeRush(
                    CommandHelper.GetCodeRushTemplateCommandData(
                        template,
                        SettingsModel.Context,
                        variablesToSet,
                        ButtonState.Up,
                        dynamicListEntries,
                        buttonInstanceId));
            }
        }

        async Task RefreshButtonImage()
        {
            using (Graphics background = GetBackground())
            {
                // Max font size for three lines: 22
                // Max font size for two lines: 30
                // Max font size for one line: 45
                // Max font size for readability: 22?

                const float buttonWidth = 144f;
                const float minFontSize = 22f;
                float fontSize = 20;
                switch (SettingsModel.Kind)
                {
                    case TypeKind.Simple:
                        fontSize = 45;
                        Font testFont = new Font(STR_FontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                        SizeF measureString = background.MeasureString(SettingsModel.SimpleType, testFont);
                        while (measureString.Width > buttonWidth)
                        {
                            fontSize--;
                            if (fontSize <= minFontSize)
                                break;
                            testFont = new Font(STR_FontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                            measureString = background.MeasureString(SettingsModel.SimpleType, testFont);
                        }
                        break;
                    case TypeKind.GenericOneTypeParameter:
                        fontSize = 30;
                        break;
                    case TypeKind.GenericTwoTypeParameters:
                        fontSize = 22;
                        break;
                }

                
                float lineHeight = fontSize * 1.2f;
                float x = 0;
                const float buttonHeight = 144f;
                float line1 = buttonHeight - lineHeight * 3;
                float line2 = buttonHeight - lineHeight * 2;
                float line3 = buttonHeight - lineHeight;

                Font font = new Font(STR_FontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

                switch (SettingsModel.Kind)
                {
                    case TypeKind.Simple:
                        
                        background.DrawString(SettingsModel.SimpleType, font, Brushes.White, x, line3);
                        break;
                    case TypeKind.GenericOneTypeParameter:
                        background.DrawString(SettingsModel.GenericType, font, Brushes.White, x, line2);
                        background.DrawString(SettingsModel.TypeParam1, font, Brushes.White, x, line3);
                        break;
                    case TypeKind.GenericTwoTypeParameters:
                        background.DrawString(SettingsModel.GenericType, font, Brushes.White, x, line1);
                        background.DrawString(SettingsModel.TypeParam1, font, Brushes.White, x, line2);
                        background.DrawString(SettingsModel.TypeParam2, font, Brushes.White, x, line3);
                        break;
                }
            }
            await DrawBackgroundAsync();
        }

        public override async void TypeRecognized(TypeRecognizedFromSpokenWords typeRecognizedFromSpokenWords)
        {
            SettingsModel.GenericType = typeRecognizedFromSpokenWords.GenericType;
            SettingsModel.SimpleType = typeRecognizedFromSpokenWords.SimpleType;
            SettingsModel.Kind = typeRecognizedFromSpokenWords.Kind;
            SettingsModel.TypeParam1 = typeRecognizedFromSpokenWords.TypeParam1;
            SettingsModel.TypeParam2 = typeRecognizedFromSpokenWords.TypeParam2;
            await Manager.SetSettingsAsync(lastContext, SettingsModel);
            await RefreshButtonImage();
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            await RefreshButtonImage();
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await RefreshButtonImage();
        }
    }
}
