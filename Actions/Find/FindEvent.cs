using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.event")]
    public class FindEvent : BaseFindSymbolAction
    {
        public FindEvent()
        {

        }

        public override string SymbolName => "event";
        public override string SpokenWordsStart => "event";
    }
}
