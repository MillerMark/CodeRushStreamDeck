using System;
using System.Linq;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace CodeRushStreamDeck
{
    [ActionUuid(Uuid = "com.devexpress.coderush.string.modifier")]
    public class StringModifierAction : BaseStreamDeckActionWithSettingsModel<Models.StringModifierModel>
    {
        string lastContext;
        public StringModifierAction()
        {
            Variables.StringVarChanged += Variables_StringVarChanged;
        }

        private async void Variables_StringVarChanged(object sender, VarEventArgs<string> e)
        {
            if (e.Name != SettingsModel.VariableName)
                return;

            if (SettingsModel.CurrentValue != e.Value)
            {
                SettingsModel.CurrentValue = e.Value;
                await Manager.SetSettingsAsync(lastContext, SettingsModel);
            }
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            SettingsModel.CurrentValue = SettingsModel.Value;

            await Manager.SetSettingsAsync(args.context, SettingsModel);
            if (!string.IsNullOrEmpty(SettingsModel.VariableName))
                Variables.SetString(SettingsModel.VariableName, SettingsModel.Value);
            await base.OnKeyDown(args);
        }

        public override Task OnWillAppear(StreamDeckEventPayload args)
        {
            lastContext = args.context;
            if (Variables.ContainsStringVar(SettingsModel.VariableName))
                SettingsModel.CurrentValue = Variables.GetString(SettingsModel.VariableName);
            return base.OnWillAppear(args);
        }
    }
}
