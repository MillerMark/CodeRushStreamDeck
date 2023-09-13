using System;
using System.Linq;

namespace CodeRushStreamDeck
{
    public class VarEventArgs<T>
    {
        public T Value { get; set; }
        public string Name { get; set; }
        public VarEventArgs(string name, T value)
        {
            Value = value;
            Name = name;
        }
        public VarEventArgs()
        {

        }
    }
}
