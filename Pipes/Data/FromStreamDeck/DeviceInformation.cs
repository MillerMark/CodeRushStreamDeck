using System.Collections.Generic;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class DeviceInformation : FromStreamDeckData
    {
        public List<StreamDeckDevice> Devices { get; set; } = new();
        public DeviceInformation()
        {

        }
    }
}
