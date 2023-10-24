using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StreamDeckLib;
using StreamDeckLib.Messages;
using CodeRushStreamDeck.Models;
using static CodeRushStreamDeck.VisualStudioCommandAction;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public class CarouselHelper
    {
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

        public static string GetImageFolder()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images\commands\vs");
        }

        public async Task<bool> HandleCommandOverride(ConnectionManager manager, StreamDeckEventPayload args, ICanSupportImageCarousel settingsModel)
        {
            string overrideCommand = settingsModel.OverrideCommand;

            if (string.IsNullOrEmpty(overrideCommand))
                return false;

            settingsModel.OverrideCommand = null;

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
                    await LoadStreamDeckImages(manager, args, settingsModel.Command, settingsModel.SearchTextForImages);
                    return true;

                case "Copy":
                    string imageFullPath = Path.Combine(GetImageFolder(), parameters + "@2x.png");
                    $"echo {imageFullPath}| clip".Bat();
                    return true;
            }

            return false;
        }

        public async Task LoadStreamDeckImages(ConnectionManager Manager, StreamDeckEventPayload args, string command, string additionalSearchPhrase = null)
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

        public async Task ShowImage(ConnectionManager manager, StreamDeckEventPayload args, ICanSupportImageCarousel settingsModel)
        {
            if (!string.IsNullOrEmpty(settingsModel.SelectedImage))
                await manager.SetImageAsync(args.context, GetImageFileName(settingsModel));
        }

        static string GetImageFileName(ICanSupportImageCarousel settingsModel)
        {
            if (settingsModel == null)
                return null;

            string imageName = settingsModel.SelectedImage;
            if (string.IsNullOrEmpty(imageName))
                imageName = settingsModel.ImageFileName;

            if (string.IsNullOrEmpty(imageName))
                return null;

            return $"images/commands/vs/{imageName}@2x.png";
        }

        public async Task OnDidReceiveSettings(ConnectionManager manager, StreamDeckEventPayload args, ICanSupportImageCarousel settingsModel)
        {
            if (!string.IsNullOrEmpty(settingsModel.OverrideCommand))
            {
                var result = await HandleCommandOverride(manager, args, settingsModel);
                if (result)
                    return;
            }

            // TODO: Asymmetry - consider adding parameter parsing for commands and updating the handleSdpiItemChange method.
            if (!string.IsNullOrEmpty(settingsModel.SelectedImage))
            {
                settingsModel.ImageFileName = settingsModel.SelectedImage;
                await ShowImage(manager, args, settingsModel);
                settingsModel.SelectedImage = string.Empty;
                return;
            }

            await LoadStreamDeckImages(manager, args, settingsModel.Command);
        }

        public Bitmap GetImage(CodeRushToggleStateCommandModel settingsModel)
        {
            string imageFileName = GetImageFileName(settingsModel);
            if (string.IsNullOrEmpty(imageFileName) || !File.Exists(imageFileName))
                return new Bitmap(144, 144);
            return new Bitmap(imageFileName);
        }
    }
}
