using System;
using System.Linq;
using System.Collections.Generic;
using StreamDeckLib.Messages;
using System.Collections.Concurrent;

namespace CodeRushStreamDeck
{
    public static class ButtonTracker
    {
        static ConcurrentDictionary<string, IStreamDeckButton> buttonInstances = new();
        public static void OnKeyDown(string buttonId, IStreamDeckButton streamDeckButton)
        {
            buttonInstances.TryAdd(buttonId, streamDeckButton);
        }

        public static void ShowAlert(string buttonID)
        {
            if (buttonInstances.TryGetValue(buttonID, out IStreamDeckButton button))
                button.ShowAlert();
        }


    }
}
