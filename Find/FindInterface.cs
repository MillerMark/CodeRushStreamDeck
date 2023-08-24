using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.interface")]
    public class FindInterface : BaseFindSymbolAction
    {
        public FindInterface()
        {

        }

        public override string SymbolName => "interface";
        public override string SpokenWordsStart => "interface";
    }
}
