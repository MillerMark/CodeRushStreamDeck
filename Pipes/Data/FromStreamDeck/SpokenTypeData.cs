using System;
using System.Collections.Generic;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class SpokenTypeData : ButtonStreamDeckData
    {
        public string Template { get; set; }
        public string Context { get; set; }
        public string VariablesToSet { get; set; }
        public SpokenTypeData()
        {
        }
    }
}
