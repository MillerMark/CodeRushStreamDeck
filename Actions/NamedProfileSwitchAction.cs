using System;
using System.Linq;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Startup;
using System.Runtime.Versioning;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.named.profile.switch")]
    public class NamedProfileSwitchAction : StreamDeckButton<Models.NamedProfileCommandModel>, IStreamDeckButton
    {
        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            if (!string.IsNullOrEmpty(SettingsModel.ProfileName))
            {
                string uuid = Manager.GetInstanceUuid();
                await Manager.SwitchToProfileAsync(uuid, args.device, SettingsModel.ProfileName);
            }
        }
    }
}
