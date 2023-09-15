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

namespace CodeRushStreamDeck
{
    [ActionUuid(Uuid = "com.devexpress.coderush.template.expand")]
    public class CodeRushTemplateExpandAction : BaseStreamDeckActionWithSettingsModel<Models.CodeRushTemplateCommandModel>
    {
        string lastContext;
        string buttonInstanceId = Guid.NewGuid().ToString();

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
        
        void ExpandCodeRushTemplateInCodeRush(string templateName, string variablesToSet, List<DynamicListEntry> dynamicListEntries, ButtonState buttonState)
        {
            string data = JsonConvert.SerializeObject(CommandHelper.GetCodeRushTemplateCommandData(templateName, variablesToSet, buttonState, dynamicListEntries, buttonInstanceId));
            CommunicationServer.SendMessageToCodeRush(data, nameof(CodeRushTemplateCommandData));
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);

            Variables.ClearDynamicListEntries();
            SettingsModel.FullTemplateName = GetFullTemplateName();
            ExpandCodeRushTemplateInCodeRush(SettingsModel.FullTemplateName, GetFullVariablesToSet(), Variables.DynamicListEntries, ButtonState.Down);
        }

        public override async Task OnPropertyInspectorDidAppear(StreamDeckEventPayload args)
        {
            Variables.ClearDynamicListEntries();
            SettingsModel.FullTemplateName = GetFullTemplateName();
            await Manager.SetSettingsAsync(args.context, SettingsModel);
            await base.OnPropertyInspectorDidAppear(args);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            lastContext = args.context;
        }
    }
}
