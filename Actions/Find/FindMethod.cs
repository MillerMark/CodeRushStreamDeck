using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.method")]
    public class FindMethod : BaseFindSymbolAction
    {
        public FindMethod()
        {

        }

        public override string SymbolName => "method";
        public override string SpokenWordsStart => "method";
    }
}
