using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    /// <summary>
    /// Helps load the specified list of string commands into an HTML data list.
    /// </summary>
    public static class CommandLoader
    {
        public static async Task LoadCodeRushCommands(ConnectionManager manager, string context)
        {
            await LoadCommands(manager, context, CodeRush.Commands);
        }

        public static async Task LoadVisualStudioCommands(ConnectionManager manager, string context)
        {
            await LoadCommands(manager, context, VisualStudio.Commands);
        }

        private static async Task LoadCommands(ConnectionManager manager, string context, List<string> commands)
        {
            dynamic obj = new JObject();
            obj.Command = "!LoadCommands";
            obj.Commands = new JArray();
            foreach (var item in commands)
                obj.Commands.Add(item);

            await manager.SendToPropertyInspectorAsync(context, obj);
        }
    }
}
