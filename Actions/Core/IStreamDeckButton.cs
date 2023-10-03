using System;
using System.Linq;

namespace CodeRushStreamDeck
{
    public interface IStreamDeckButton
    {
        void ShowAlert();
        string Id { get; }
    }
}
