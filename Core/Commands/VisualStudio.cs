using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeRushStreamDeck
{
    public static class VisualStudio
    {
        public static event EventHandler CommandsInitialized;
        public static List<string> Commands { get; set; }
        public static bool IsInitialized => Commands != null;
        public static void SetCommands(List<string> commands)
        {
            Commands = commands.Where(x => !x.Contains("ContextMenus.")).Order().Distinct().ToList();
            if (commands != null)
                CommandsInitialized?.Invoke(null, EventArgs.Empty);
        }

        public static void GetDataIfNeeded()
        {
            if (!StreamDeck.CommandsExist)
                StreamDeck.RequestCommands();
        }
    }
}
