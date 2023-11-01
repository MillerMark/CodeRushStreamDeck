using System.Collections.Generic;
using System.Runtime.Versioning;

namespace CodeRushStreamDeck.Models
{
    [SupportedOSPlatform("windows")]
    public class CodeRushToggleStateCommandModel : CodeRushCommandModel
    {
        public string StateName { get; set; }
    }
}
