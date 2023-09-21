using System;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public abstract class StreamDeckButton<T> : BaseStreamDeckActionWithSettingsModel<T>, IStreamDeckButton
    {
        protected string lastContext;
        protected string buttonInstanceId = Guid.NewGuid().ToString();
        public string Id => buttonInstanceId;

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            lastContext = args.context;
            ButtonTracker.OnKeyDown(this);
        }

        public override async Task OnKeyUp(StreamDeckEventPayload args)
        {
            await base.OnKeyUp(args);
            ButtonTracker.OnKeyUp(this);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            lastContext = args.context;
        }

        public async void ShowAlert()
        {
            await Manager.ShowAlertAsync(lastContext);
        }

    }
}
