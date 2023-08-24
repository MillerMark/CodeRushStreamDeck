using StreamDeckLib;
using System;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.find.class")]
    public class FindClass: BaseFindSymbolAction
    {
        public FindClass()
        {

        }

        public override string SymbolName => "class";
    }
}
