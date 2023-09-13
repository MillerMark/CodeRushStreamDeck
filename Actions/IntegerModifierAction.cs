using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace CodeRushStreamDeck
{

[ActionUuid(Uuid = "com.devexpress.coderush.integer.modifier")]
    public class IntegerModifierAction : BaseStreamDeckActionWithSettingsModel<Models.IntegerModifierModel>
    {

        string lastContext;

        public IntegerModifierAction()
        {
            Variables.IntVarChanged += Variables_IntVarChanged;
        }

        private async void Variables_IntVarChanged(object sender, VarEventArgs<int> e)
        {
            if (e.Name != SettingsModel.VariableName)
                return;

            if (SettingsModel.Value != e.Value)
            {
                SettingsModel.Value = e.Value;
                await Manager.SetSettingsAsync(lastContext, SettingsModel);
            }
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            int newValue = SettingsModel.Value + SettingsModel.Delta;
            if (newValue > SettingsModel.Max)
            {
                newValue = SettingsModel.Max;
                if (newValue == SettingsModel.Value)
                    ShowAlert(); // No change and out of bounds!!
            }

            if (newValue < SettingsModel.Min)
            {
                newValue = SettingsModel.Min;
                if (newValue == SettingsModel.Value)
                    ShowAlert(); // No change and out of bounds!!
            }

            SettingsModel.Value = newValue;

            await Manager.SetSettingsAsync(args.context, SettingsModel);
            if (!string.IsNullOrEmpty(SettingsModel.VariableName))
                Variables.SetInt(SettingsModel.VariableName, SettingsModel.Value);
            await base.OnKeyDown(args);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            lastContext = args.context;
            if (Variables.ContainsIntVar(SettingsModel.VariableName))
                SettingsModel.Value = Variables.GetInt(SettingsModel.VariableName);
        }

        public override Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            return base.OnDidReceiveSettings(args);
        }

        public async void ShowAlert()
        {
            await Manager.ShowAlertAsync(lastContext);
        }
    }
}
