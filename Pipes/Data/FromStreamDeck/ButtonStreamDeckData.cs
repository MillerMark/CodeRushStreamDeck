using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.CodeRush.Foundation.Pipes.Data {
    public class ButtonStreamDeckData : FromStreamDeckData
    {
        public string ButtonId { get; set; }
        public ButtonState ButtonState { get; set; }
    }
}
