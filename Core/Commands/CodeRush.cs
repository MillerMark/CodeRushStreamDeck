using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeRushStreamDeck
{
    public static class CodeRush
    {
        public static event EventHandler CommandsInitialized;
        public static List<string> Commands { get; set; }
        public static bool IsInitialized => Commands != null;
        public static void SetCommands(List<string> commands)
        {
            Commands = commands;
            if (commands != null)
                CommandsInitialized?.Invoke(null, EventArgs.Empty);
        }
    }
}
