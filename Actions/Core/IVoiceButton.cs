using System;
using DevExpress.CodeRush.Foundation.Pipes.Data;

namespace CodeRushStreamDeck
{
    public interface IVoiceButton
    {
        void ListeningStarted();

        /// <summary>
        /// Update the volume on the Stream Deck button. <paramref name="volume"/> will be a value between 0 and 7.
        /// </summary>
        /// <param name="volume">A value representing the volume of the sound detected at the microphone. Will be a 
        /// value between 0 and 7</param>
        void UpdateVolume(int volume);

        void TypeRecognized(TypeRecognizedFromSpokenWords typeRecognizedFromSpokenWords);

    }
}
