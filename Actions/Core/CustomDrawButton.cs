using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using StreamDeckLib;
using StreamDeckLib.Messages;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using PipeCore;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public abstract class CustomDrawButton<T> : StreamDeckButton<T>
    {
        public float IconScale { get; set; } = 1.0f;
        public float IconTop { get; set; } = 0f;
        public float IconLeft { get; set; } = 0f;
        private const int iconSize = 144;
        protected static object systemDrawingLock = new object();
        protected Bitmap backgroundImage;
        protected abstract string BackgroundImageName { get; }

        protected virtual void DrawBackgroundBehindImage(Graphics graphics)
        {
            // Do nothing. Let descendants override if they want to draw a background behind the icon image.
        }

        protected virtual Bitmap GetIconImage()
        {
            return GetBitmapResource(BackgroundImageName);
        }

        protected Graphics GetBackground()
        {
            backgroundImage = new Bitmap(iconSize, iconSize);
            Graphics graphics = Graphics.FromImage(backgroundImage);

            DrawBackgroundBehindImage(graphics);
            Bitmap iconImage = GetIconImage();
            graphics.DrawImage(iconImage, IconLeft, IconTop, IconScale * iconSize, IconScale * iconSize);
            return graphics;
        }

        protected Bitmap GetBitmapResource(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new Bitmap(iconSize, iconSize);

            Stream manifestResourceStream = GetType().Assembly.GetManifestResourceStream($"CodeRushStreamDeck.images.resources.{name}@2x.png");
            
            if (manifestResourceStream == null)
                return new Bitmap(iconSize, iconSize);

            return new Bitmap(manifestResourceStream);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);

            await UpdateImageAsync();
        }

        protected virtual void RefreshButtonImage(Graphics background)
        {
            // Do nothing. Let descendants override.
        }

        string GetImageLocation()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                backgroundImage.Save(stream, ImageFormat.Png);

                byte[] imageBytes = stream.ToArray();
                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
            }
        }
        protected async Task UpdateImageAsync()
        {
            string imageLocation;
            lock (systemDrawingLock)
            {
                using (Graphics background = GetBackground())
                    RefreshButtonImage(background);
                
                imageLocation = GetImageLocation();
            }

            await Manager.SetImageAsync(lastContext, imageLocation);
        }
    }
}
