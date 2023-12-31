﻿using System;
using System.Drawing;
using System.Runtime.Versioning;
using PipeCore;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using Pipes.Server;
using StreamDeckLib.Messages;
using System.Threading.Tasks;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public abstract class VoiceButton<T> : CustomDrawButton<T>, IVoiceButton
    {
        static Bitmap listeningIcon;
        static Bitmap readyIcon;
        static Bitmap waitingIcon;
        ListeningState listeningState;
        int lastVolume;
        const int recordLeftOffset = 2;
        const int recordTopOffset = 2;
        protected bool heardAudio;

        public ListeningState ListeningState
        {
            get => listeningState;
            set
            {
                if (listeningState == value)
                    return;
                listeningState = value;
                UpdateImageAsync().FireAndForget();
            }
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            heardAudio = false;
            await base.OnKeyDown(args);
            ListeningState = ListeningState.Waiting;
        }

        void LoadIconsIfNeeded()
        {
            if (readyIcon == null)
                readyIcon = GetBitmapResource("VoiceReady");
            if (waitingIcon == null)
                waitingIcon = GetBitmapResource("VoiceWaiting");
            if (listeningIcon == null)
                listeningIcon = GetBitmapResource("VoiceListening");
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            await base.OnKeyUp(args);
            ListeningState = ListeningState.Ready;
            SendCommandToCodeRush(CommandsFromStreamDeck.StopListening, ButtonState.Up);
            lastVolume = 0;
        }

        void ShowActiveVolume(Graphics background)
        {
            int streamDeckVolume = (int)(9 * (lastVolume - 1) / 100);  // Gets a volume from 0-8.
            int inBoundsVolume = Math.Min(8, Math.Max(0, streamDeckVolume));
            const int rectWidth = 8;
            const int rectHeight = 20;
            const int recordDiameter = 32;
            const int horizontalGap = 4;
            const int volumeTop = recordTopOffset + (recordDiameter - rectHeight) / 2 - 1;
            const int barStartX = recordLeftOffset + recordDiameter + horizontalGap + 1;
            for (int i = 1; i <= inBoundsVolume; i++)
            {
                int x = barStartX + (horizontalGap + rectWidth) * (i - 1);
                Rectangle rect = new Rectangle() { Height = rectHeight, Width = rectWidth, Y = volumeTop, X = x };
                background.FillRectangle(Brushes.Red, rect);
            }
        }

        private void AddListeningState(Graphics background)
        {
            LoadIconsIfNeeded();
            Image icon;
            if (ListeningState == ListeningState.Listening)
                icon = listeningIcon;
            else if (ListeningState == ListeningState.Waiting)
                icon = waitingIcon;
            else
                icon = readyIcon;
            background.DrawImage(icon, new Point(recordLeftOffset, recordTopOffset));

            if (ListeningState == ListeningState.Listening)
                ShowActiveVolume(background);
        }

        protected override void RefreshButtonImage(Graphics background)
        {
            AddListeningState(background);
        }

        public void ListeningStarted()
        {
            ListeningState = ListeningState.Listening;
        }

        public async void UpdateVolume(int volume)
        {
            lastVolume = volume;
            if (ListeningState == ListeningState.Waiting)
                ListeningState = ListeningState.Listening;  // Will trigger a button image refresh
            else
                await UpdateImageAsync();
            if (ListeningState == ListeningState.Listening)
                if (volume >= 8)
                    heardAudio = true;
        }

        public virtual void TypeRecognized(TypeRecognizedFromSpokenWords typeRecognizedFromSpokenWords)
        {
            // Do nothing. Let descendants override if needed.
        }
    }
}
