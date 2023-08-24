using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.field")]
    public class FindField : BaseFindSymbolAction
    {
        public FindField()
        {

        }

        public override string SymbolName => "field";
        public override string SpokenWordsStart => "field";
    }
}
