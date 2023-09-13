using System;
using System.Linq;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Models;

namespace CodeRushStreamDeck
{
    [ActionUuid(Uuid = "com.devexpress.coderush.string.modifier")]
    public class StringModifierAction : BaseStreamDeckActionWithSettingsModel<Models.StringModifierModel>
    {
        string lastContext;
        string lastColor;
        bool lastSelected;
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

            if (SelectionStateHasChanged())
                await LoadImage(lastContext);
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            SettingsModel.CurrentValue = SettingsModel.Value;

            await Manager.SetSettingsAsync(args.context, SettingsModel);
            if (!string.IsNullOrEmpty(SettingsModel.VariableName))
                if (Variables.GetString(SettingsModel.VariableName) == SettingsModel.Value)
                {
                    if (SettingsModel.AllowZeroSelected)  // Toggle (clear) the value.
                    {
                        SettingsModel.CurrentValue = string.Empty;
                        Variables.SetString(SettingsModel.VariableName, string.Empty);
                    }
                }
                else
                    Variables.SetString(SettingsModel.VariableName, SettingsModel.Value);

            if (SelectionStateHasChanged())
                await LoadImage(args.context);
        }

        private bool SelectionStateHasChanged()
        {
            return lastSelected != IsSelected();
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            lastContext = args.context;
            if (Variables.ContainsStringVar(SettingsModel.VariableName))
                SettingsModel.CurrentValue = Variables.GetString(SettingsModel.VariableName);
            await LoadImage(args.context);
        }

        async Task LoadImage(string context)
        {
            lastColor = SettingsModel.Color;
            string selected = string.Empty;

            // TODO: Determine how to best deal with image size.
            string size = "@2x";

            lastSelected = IsSelected();
            if (lastSelected)
                selected = "Selected";
            await Manager.SetImageAsync(context, $"images/controls/radioButton{SettingsModel.Color}{selected}{size}.png");
        }

        private bool IsSelected()
        {
            if (string.IsNullOrEmpty(SettingsModel.CurrentValue) && string.IsNullOrEmpty(SettingsModel.Value))
                return false;
            return SettingsModel.CurrentValue == SettingsModel.Value;
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);
            if (SettingsModel.Color != lastColor)
                await LoadImage(args.context);
        }
    }
}
