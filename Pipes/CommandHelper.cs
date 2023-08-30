using System;
using DevExpress.CodeRush.Foundation.Pipes.Data;

namespace PipeCore
{
    public static class CommandHelper
    {
        public static void InitializeCommandData(ButtonStreamDeckData buttonData, string id)
        {
            buttonData.StreamDeckPluginVersion_Major = Version.Major;
            buttonData.StreamDeckPluginVersion_Minor = Version.Minor;
            buttonData.ButtonId = id;
        }

        public static CommandData GetCommandData(string command, ButtonState buttonState, string id)
        {
            CommandData commandData = new CommandData() { Command = command };
            commandData.ButtonState = buttonState;
            InitializeCommandData(commandData, id);
            return commandData;
        }

        public static VisualStudioCommandData GetVisualStudioCommandData(string command, string parameters, ButtonState buttonState, string id)
        {
            VisualStudioCommandData visualStudioCommandData = new VisualStudioCommandData() { Command = command };
            visualStudioCommandData.ButtonState = buttonState;
            visualStudioCommandData.Parameters = parameters;
            InitializeCommandData(visualStudioCommandData, id);
            return visualStudioCommandData;
        }
    }
}
