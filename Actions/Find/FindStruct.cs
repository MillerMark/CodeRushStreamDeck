using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.struct")]
    public class FindStruct : BaseFindSymbolAction
    {
        public FindStruct()
        {

        }

        public override string SymbolName => "struct";
        public override string SpokenWordsStart => "struct";
    }
}
