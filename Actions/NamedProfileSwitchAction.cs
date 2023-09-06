using System;
using System.Linq;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Startup;

namespace CodeRushStreamDeck
{
    [ActionUuid(Uuid = "com.devexpress.coderush.named.profile.switch")]
    public class NamedProfileSwitchAction : BaseStreamDeckActionWithSettingsModel<Models.NamedProfileCommandModel>, IStreamDeckButton
    {
        string lastContext;
        protected string buttonInstanceId = Guid.NewGuid().ToString();
        public NamedProfileSwitchAction()
        {
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            lastContext = args.context;
            await base.OnKeyDown(args);
            ButtonTracker.OnKeyDown(buttonInstanceId, this);
            if (!string.IsNullOrEmpty(SettingsModel.ProfileName))
            {
                string uuid = Manager.GetInstanceUuid();
                await Manager.SwitchToProfileAsync(uuid, args.device, SettingsModel.ProfileName);
            }
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            // SettingsModel.ProfileName
        }

        public async void ShowAlert()
        {
            await Manager.ShowAlertAsync(lastContext);
        }
    }
}
