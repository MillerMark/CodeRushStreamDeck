using System;
using System.Linq;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class FromStreamDeckData
    {
        public bool RequiresActiveVisualStudio { get; set; } = true;
        public int StreamDeckPluginVersion_Major { get; set; }
        public int StreamDeckPluginVersion_Minor { get; set; }
    }
}
