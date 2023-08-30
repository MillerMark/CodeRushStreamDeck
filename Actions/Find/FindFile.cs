using System;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.file")]
    public class FindFile : BaseFindSymbolAction
    {
        public FindFile()
        {

        }

        public override string SymbolName => "file";
        public override string SpokenWordsStart => "open";
    }
}
