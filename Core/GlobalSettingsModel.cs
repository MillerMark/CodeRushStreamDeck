using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeRushStreamDeck
{
    public class GlobalSettingsModel
    {
        public List<string> Integers { get; set; } = new();
        public List<string> Strings { get; set; } = new();
        public List<string> Booleans { get; set; } = new();

        public GlobalSettingsModel()
        {

        }
    }
}
