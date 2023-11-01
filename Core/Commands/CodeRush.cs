using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeRushStreamDeck
{
    public static class CodeRush
    {
        public static List<string> Commands { get; set; }
        public static bool IsInitialized => Commands != null;
        public static void SetCommands(List<string> commands)
        {
            Commands = commands;
        }
    }
}
