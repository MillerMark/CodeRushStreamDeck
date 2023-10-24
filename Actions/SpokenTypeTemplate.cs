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
using System.Diagnostics.Metrics;
using System.Reflection;

namespace CodeRushStreamDeck
{

    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.spoken.type.template")]
    public class SpokenTypeTemplate : VoiceButton<Models.SpokenTypeTemplateData>
    {
        DateTime keyDownTime;

        ScrollingText scrollingText;

        protected override string BackgroundImageName
        {
            get
            {
                if (SettingsModel.TemplateToExpand.StartsWith("m"))
                {
                    return "AddMethod";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("v"))
                {
                    return "AddField";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("p"))
                {
                    return "AddProperty";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("ev"))
                {
                    return "AddEvent";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("i"))
                {
                    return "AddInterface";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("c"))
                {
                    return "AddClass";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("s"))
                {
                    return "AddStruct";
                }
                if (SettingsModel.TemplateToExpand.StartsWith("e"))
                {
                    return "AddEnum";
                }
                return "AddMethod";
            }
        }

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
            keyDownTime = DateTime.UtcNow;
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
            var totalKeyDownTimeSeconds = (DateTime.UtcNow - keyDownTime).TotalSeconds;

            await base.OnKeyUp(args);
            if (!heardAudio || totalKeyDownTimeSeconds < 0.5)
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
            await UpdateImageAsync();
        }

        public override async void TypeRecognized(TypeRecognizedFromSpokenWords typeRecognizedFromSpokenWords)
        {
            SettingsModel.GenericType = typeRecognizedFromSpokenWords.GenericType;
            SettingsModel.SimpleType = typeRecognizedFromSpokenWords.SimpleType;
            SettingsModel.Kind = typeRecognizedFromSpokenWords.Kind;
            SettingsModel.TypeParam1 = typeRecognizedFromSpokenWords.TypeParam1;
            SettingsModel.TypeParam2 = typeRecognizedFromSpokenWords.TypeParam2;
            await Manager.SetSettingsAsync(lastContext, SettingsModel);
            scrollingText?.InvalidateDrawingParameters();
            await UpdateImageAsync();
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            scrollingText?.InvalidateDrawingParameters();
            await UpdateImageAsync();
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await UpdateImageAsync();
        }
    }
}
