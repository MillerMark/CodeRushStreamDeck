using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.enum")]
    public class FindEnum : BaseFindSymbolAction
    {
        public FindEnum()
        {

        }

        public override string SymbolName => "enum";
        public override string SpokenWordsStart => "enum";
    }
}
