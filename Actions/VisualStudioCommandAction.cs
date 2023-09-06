using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using Newtonsoft.Json;
using Pipes.Server;
using PipeCore;
using CodeRushStreamDeck.Startup;
using System.Diagnostics;

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.exec.vs.command")]
    public class VisualStudioCommandAction : BaseStreamDeckActionWithSettingsModel<Models.VisualStudioCommandModel>, IStreamDeckButton
    {
        protected string buttonInstanceId = Guid.NewGuid().ToString();
        public VisualStudioCommandAction()
        {
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            lastContext = args.context;
            await base.OnKeyDown(args);
            ButtonTracker.OnKeyDown(buttonInstanceId, this);
            if (!string.IsNullOrEmpty(SettingsModel.Command))
            {
                SendVisualStudioCommandToCodeRush(SettingsModel.Command, SettingsModel.Parameters, ButtonState.Down);
            }
            else
            {
                // TODO: Remove this test code:
                string uuid = Manager.GetInstanceUuid();

                const string xlRight = "EA7156E07669CF0850A6A747AE71EC5E";
                const string profileName = "VS Debug";
                await Manager.SwitchToProfileAsync(uuid, xlRight, profileName);
            }
        }

        public class ImageFileNameScore
        {
            public string FileName { get; set; }
            public double Score { get; set; }
            public ImageFileNameScore(string fileName, double score)
            {
                FileName = fileName;
                Score = score;
            }
        }

        async Task ShowImage(StreamDeckEventPayload args)
        {
            if (!string.IsNullOrEmpty(SettingsModel.SelectedImage))
                await Manager.SetImageAsync(args.context, $"images/commands/vs/{SettingsModel.SelectedImage}.png");
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await ShowImage(args);
        }

        void SendVisualStudioCommandToCodeRush(string command, string parameters, ButtonState buttonState)
        {
            string data = JsonConvert.SerializeObject(CommandHelper.GetVisualStudioCommandData(command, parameters, buttonState, buttonInstanceId));
            CommunicationServer.SendMessageToCodeRush(data, nameof(VisualStudioCommandData));
        }

        string GetImageFolder()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\commands\vs");
        }

        async Task HandleCommandOverride(StreamDeckEventPayload args)
        {
            string overrideCommand = SettingsModel.OverrideCommand;
            if (string.IsNullOrEmpty(overrideCommand))
                return;

            string command;
            string parameters = string.Empty;
            int spaceIndex = overrideCommand.IndexOf(" ");
            if (spaceIndex > 0)
            {
                command = overrideCommand.Substring(0, spaceIndex);
                parameters = overrideCommand.Substring(spaceIndex + 1);
            }
            else
                command = overrideCommand;

            switch (command)
            {
                case "FindImages":
                    await LoadStreamDeckImages(args, SettingsModel.Command, SettingsModel.SearchTextForImages);
                    commandHandled = true;
                    break;

                case "Copy":
                    string imageFullPath = Path.Combine(GetImageFolder(), parameters + "@2x.png");
                     $"echo {imageFullPath}| clip".Bat();
                    commandHandled = true;
                    break;
            }
            SettingsModel.OverrideCommand = null;
        }

        bool commandHandled;
        string lastContext;
        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);

            commandHandled = false;
            if (!string.IsNullOrEmpty(SettingsModel.OverrideCommand))
                await HandleCommandOverride(args);

            if (commandHandled)
            {
                commandHandled = false;
                return;
            }

            // TODO: Assymetry - consider adding parameter parsing for commands and updating the handleSdpiItemChange method.
            if (!string.IsNullOrEmpty(SettingsModel.SelectedImage))
            {
                SettingsModel.ImageFileName = SettingsModel.SelectedImage;
                await ShowImage(args);
                // TODO: Load selected image...
                SettingsModel.SelectedImage = string.Empty;
                await Manager.SetSettingsAsync("ImageFileName", SettingsModel.ImageFileName);
                return;
            }

            await LoadStreamDeckImages(args, SettingsModel.Command);

            // TODO: Show the images from this topTen list.
        }

        private async Task LoadStreamDeckImages(StreamDeckEventPayload args, string command, string additionalSearchPhrase = null)
        {
            List<string> wordParts = CamelCaseParser.GetWordParts(command);
            List<string> additionalSearchPhraseWordParts = CamelCaseParser.GetWordParts(additionalSearchPhrase);

            string[] vsCommandImageFileNames = Directory.GetFiles(GetImageFolder(), "*@2x.png");
            List<ImageFileNameScore> list = new List<ImageFileNameScore>();
            foreach (string vsCommandImageFileName in vsCommandImageFileNames)
            {
                string fileNameOnly = Path.GetFileName(vsCommandImageFileName);
                string baseFileName = fileNameOnly.Substring(0, fileNameOnly.Length - 7);
                List<string> imageWordParts = CamelCaseParser.GetWordParts(baseFileName);
                double score = MatchScoreCalculator.GetScore(wordParts, imageWordParts);

                if (additionalSearchPhraseWordParts != null)
                    score = Math.Max(score, MatchScoreCalculator.GetScore(additionalSearchPhraseWordParts, imageWordParts));

                if (score > 0.1)
                    list.Add(new ImageFileNameScore(baseFileName, score));
            }

            var top14 = list.OrderByDescending(x => x.Score).Take(14).ToList();

            dynamic obj = new JObject();
            obj.Command = "!SuggestedImageList";
            obj.Images = new JArray();
            foreach (var item in top14)
                obj.Images.Add(item.FileName);

            await Manager.SendToPropertyInspectorAsync(args.context, obj);
        }

        public async void ShowAlert()
        {
            await Manager.ShowAlertAsync(lastContext);
        }
    }
}
