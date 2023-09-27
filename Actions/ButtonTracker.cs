using System;
using System.Linq;
using System.Collections.Generic;
using StreamDeckLib.Messages;
using System.Collections.Concurrent;

namespace CodeRushStreamDeck
{
    public static class ButtonTracker
    {
        /// <summary>
        /// The Stream Deck buttons that are currently down.
        /// </summary>
        static ConcurrentDictionary<string, IStreamDeckButton> buttonsDown = new();

        /// <summary>
        /// All known Stream Deck buttons that have been pressed.
        /// </summary>
        static ConcurrentDictionary<string, IStreamDeckButton> buttonInstances = new();

        public static void OnKeyDown(IStreamDeckButton button)
        {
            buttonInstances.TryAdd(button.Id, button);
            buttonsDown.TryAdd(button.Id, button);
        }

        public static void ShowAlert(string buttonID)
        {
            if (buttonInstances.TryGetValue(buttonID, out IStreamDeckButton button))
                button.ShowAlert();
        }

        public static void OnKeyUp(IStreamDeckButton button)
        {
            buttonsDown.Remove(button.Id, out _);
        }

        public static IStreamDeckButton GetDown(string buttonId)
        {
            if (buttonsDown.TryGetValue(buttonId, out IStreamDeckButton button))
                return button;
            return null;
        }

        public static IStreamDeckButton Get(string buttonId)
        {
            if (buttonInstances.TryGetValue(buttonId, out IStreamDeckButton button))
                return button;
            return null;
        }
    }
}
