using System;
using DevExpress.CodeRush.Foundation.Pipes.Data;

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

        public static VisualStudioCommandData GetVisualStudioCommandData(string command, string parameters, ButtonState buttonState, string id)
        {
            VisualStudioCommandData visualStudioCommandData = new VisualStudioCommandData() { Command = command };
            visualStudioCommandData.ButtonState = buttonState;
            visualStudioCommandData.Parameters = parameters;
            InitializeButtonData(visualStudioCommandData, id);
            return visualStudioCommandData;
        }

        public static DeviceInformation GetDeviceInformation()
        {
            DeviceInformation deviceInformation = new DeviceInformation();
            InitializeFromStreamDeckData(deviceInformation);
            return deviceInformation;
        }
    }
}
