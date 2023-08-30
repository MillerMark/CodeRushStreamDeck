using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.property")]
    public class FindProperty : BaseFindSymbolAction
    {
        public FindProperty()
        {

        }

        public override string SymbolName => "property";
        public override string SpokenWordsStart => "property";
    }
}
