namespace DevExpress.CodeRush.Foundation.Pipes.Data
{
    public class CommandData : ButtonStreamDeckData
    {
        public string Command { get; set; }
        public bool RequiresActiveVisualStudio { get; set; } = true;
        public CommandData()
        {
        }
    }
}
