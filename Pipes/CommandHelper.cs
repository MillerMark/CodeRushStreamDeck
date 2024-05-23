using System;
using System.Collections.Generic;
using CodeRushStreamDeck;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using DevExpress.CodeRush.Foundation.Templates.Shared;

namespace PipeCore
{
    public static class CommandHelper
    {
        public static void InitializeButtonData(ButtonStreamDeckData buttonData, string buttonInstanceId)
        {
            InitializeFromStreamDeckData(buttonData); 
            buttonData.ButtonId = buttonInstanceId;
        }

        private static void InitializeFromStreamDeckData(FromStreamDeckData commandData)
        {
            commandData.StreamDeckPluginVersion_Major = Version.Major;
            commandData.StreamDeckPluginVersion_Minor = Version.Minor;
        }

        public static CommandData GetCommandData(string command, ButtonState buttonState, string buttonInstanceId)
        {
            CommandData commandData = new CommandData() { Command = command };
            commandData.ButtonState = buttonState;
            InitializeButtonData(commandData, buttonInstanceId);
            return commandData;
        }

        public static SpokenTypeData GetSpokenTypeData(string template, string context, string variablesToSet, ButtonState buttonState, string id)
        {
            SpokenTypeData spokenTypeData = new SpokenTypeData() { 
                Template = template, 
                Context = context, 
                VariablesToSet = variablesToSet,
                ButtonState = buttonState 
            };

            InitializeButtonData(spokenTypeData, id);
            return spokenTypeData;
        }

        public static VisualStudioCommandData GetVisualStudioCommandData(string command, string parameters, ButtonState buttonState, string id)
        {
            VisualStudioCommandData visualStudioCommandData = new VisualStudioCommandData() { Command = command };
            visualStudioCommandData.ButtonState = buttonState;
            visualStudioCommandData.Parameters = parameters;
            InitializeButtonData(visualStudioCommandData, id);
            return visualStudioCommandData;
        }

        public static CodeRushTemplateCommandData GetCodeRushTemplateCommandData(string templateName, string context, string variablesToSet, ButtonState buttonState, List<DynamicListEntry> dynamicListEntries, string id)
        {
            CodeRushTemplateCommandData codeRushTemplateCommandData = new CodeRushTemplateCommandData();
            codeRushTemplateCommandData.ButtonState = buttonState;
            codeRushTemplateCommandData.VariablesToSet = variablesToSet;
            codeRushTemplateCommandData.TemplateName = templateName;
            codeRushTemplateCommandData.Context = context;
            codeRushTemplateCommandData.DynamicListEntries = dynamicListEntries;
            InitializeButtonData(codeRushTemplateCommandData, id);
            return codeRushTemplateCommandData;
        }

        public static CodeRushCommandData GetCodeRushCommandData(string command, string parameters, string context, ButtonState buttonState, string id)
        {
            CodeRushCommandData codeRushCommandData = new CodeRushCommandData();
            codeRushCommandData.ButtonState = buttonState;
            codeRushCommandData.Command = command;
            codeRushCommandData.Parameters = parameters;
            codeRushCommandData.Context = context;
            InitializeButtonData(codeRushCommandData, id);
            return codeRushCommandData;
        }

        public static DeviceInformation GetDeviceInformation()
        {
            DeviceInformation deviceInformation = new DeviceInformation();
            InitializeFromStreamDeckData(deviceInformation);
            return deviceInformation;
        }
    }
}
