using System;

namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class SpokenTypeData : ButtonStreamDeckData
    {
        public string Template { get; set; }
        public string Context { get; set; }
        public SpokenTypeData()
        {
        }

        public string CreateTemplate(TypeInformation typeInformation) {
            // TODO: Implement this!
            return Template;
        }
    }
}
