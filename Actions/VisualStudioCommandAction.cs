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

namespace CodeRushStreamDeck
{
    // We have to here                                | << String end quotes must end before this pipe symbol (31 characters)
    [ActionUuid(Uuid = "com.devex.cr.exec.vs.command")]
    public class VisualStudioCommandAction : BaseStreamDeckActionWithSettingsModel<Models.VisualStudioCommandModel>
    {
        protected string id = Guid.NewGuid().ToString();
        public VisualStudioCommandAction()
        {
        }

        public override async Task OnKeyDown(StreamDeckEventPayload args)
        {
            await base.OnKeyDown(args);
            if (!string.IsNullOrEmpty(SettingsModel.Command)) {
                SendVisualStudioCommandToCodeRush(SettingsModel.Command, SettingsModel.Parameters, ButtonState.Down);
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
            if (SettingsModel.SelectedImage != null)
                await Manager.SetImageAsync(args.context, $"images/commands/vs/{SettingsModel.SelectedImage}.png");
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);
            await ShowImage(args);
        }

        void SendVisualStudioCommandToCodeRush(string command, string parameters, ButtonState buttonState)
        {
            string data = JsonConvert.SerializeObject(CommandHelper.GetVisualStudioCommandData(command, parameters, buttonState, id));
            CommunicationServer.SendMessageToCodeRush(data, nameof(VisualStudioCommandData));
        }

        public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            await base.OnDidReceiveSettings(args);

            if (!string.IsNullOrEmpty(SettingsModel.SelectedImage))
            {
                SettingsModel.ImageFileName = SettingsModel.SelectedImage;
                await ShowImage(args);
                // TODO: Load selected image...
                SettingsModel.SelectedImage = string.Empty;
                await Manager.SetSettingsAsync("ImageFileName", SettingsModel.ImageFileName);
                return;
            }

            List<string> wordParts = CamelCaseParser.GetWordParts(SettingsModel.Command);

            string[] vsCommandImageFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"images\commands\vs", "*@2x.png");
            List<ImageFileNameScore> list = new List<ImageFileNameScore>();
            foreach (string vsCommandImageFileName in vsCommandImageFileNames)
            {
                string fileNameOnly = Path.GetFileName(vsCommandImageFileName);
                string baseFileName = fileNameOnly.Substring(0, fileNameOnly.Length - 7);
                List<string> imageWordParts = CamelCaseParser.GetWordParts(baseFileName);
                double score = MatchScoreCalculator.GetScore(wordParts, imageWordParts);
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

            // TODO: Show the images from this topTen list.
        }
    }
}
