using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Models;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using PipeCore;
using Pipes.Server;
using System.Runtime.Versioning;
using DevExpress.CodeRush.Foundation.Templates.Shared;

namespace CodeRushStreamDeck
{

    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.template.expand")]
    public class CodeRushTemplateExpandAction : CustomDrawButton<CodeRushTemplateCommandModel>
    {
        protected override string BackgroundImageName => "CodeRushTemplate";

        public CodeRushTemplateExpandAction()
        {
            Variables.StringVarChanged += Variables_StringVarChanged;
        }

        string GetFullTemplateName()
        {
            return Variables.Expand(SettingsModel.TemplateToExpand);
        }

        string GetFullVariablesToSet()
        {
            return Variables.Expand(SettingsModel.VariablesToSet);
        }
        
        private async void Variables_StringVarChanged(object sender, VarEventArgs<string> e)
        {
            Variables.ClearDynamicListEntries();
            string newFullTemplateName = GetFullTemplateName();
            if (SettingsModel.FullTemplateName != newFullTemplateName)
            {
                SettingsModel.FullTemplateName = newFullTemplateName;
                await Manager.SetSettingsAsync(lastContext, SettingsModel);
            }
        }
        
        void ExpandCodeRushTemplateInCodeRush(string templateName, string variablesToSet, string context, List<DynamicListEntry> dynamicListEntries, ButtonState buttonState)
        {
            CommunicationServer.SendMessageToCodeRush(CommandHelper.GetCodeRushTemplateCommandData(templateName, context, variablesToSet, buttonState, dynamicListEntries, buttonInstanceId));
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            Variables.ClearDynamicListEntries();
            SettingsModel.FullTemplateName = GetFullTemplateName();
            ExpandCodeRushTemplateInCodeRush(SettingsModel.FullTemplateName, GetFullVariablesToSet(), SettingsModel.Context,  Variables.DynamicListEntries, ButtonState.Down);
        }

        public override async Task OnPropertyInspectorDidAppear(StreamDeckEventPayload args)
        {
            Variables.ClearDynamicListEntries();
            SettingsModel.FullTemplateName = GetFullTemplateName();
            await Manager.SetSettingsAsync(args.context, SettingsModel);
            await base.OnPropertyInspectorDidAppear(args);
        }
    }
}
