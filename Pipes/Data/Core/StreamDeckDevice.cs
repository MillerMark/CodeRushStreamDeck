using System;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class StreamDeckSize {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public StreamDeckSize()
        {
            
        }
    }
    public class StreamDeckDevice
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int Type { get; set; }
        public StreamDeckSize Size { get; set; }
        public StreamDeckDevice()
        {

        }

        string GetStreamDeckType(int type) {
            switch(type)
            {
                case 0: return "Stream Deck";
                case 1: return "Stream Deck Mini";
                case 2: return "Stream Deck XL";
                case 3: return "Stream Deck Mobile";
                case 4: return "Corsair G Keys";
                case 5: return "Stream Deck Pedal";
                case 6: return "Corsair Voyager";
                case 7: return "Stream Deck Plus";
            }
            return "Unknown Model";
        }

        public override string ToString()
        {
            if (Size == null)
                return $"\"{Name}\" - {GetStreamDeckType(Type)}";
            else
                return $"\"{Name}\" - {GetStreamDeckType(Type)} ({Size.Columns}x{Size.Rows})";
        }
    }
}
