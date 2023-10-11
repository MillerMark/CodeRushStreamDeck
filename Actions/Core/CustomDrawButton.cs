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
        object systemDrawingLock = new object();
        protected Bitmap backgroundImage;
        protected abstract string BackgroundImageName { get; }
        
        protected virtual Graphics GetBackground()
        {
            lock (systemDrawingLock)
            {
                backgroundImage = GetBitmapResource(BackgroundImageName);
                return Graphics.FromImage(backgroundImage);
            }
        }

        protected Bitmap GetBitmapResource(string name)
        {
            if (string.IsNullOrEmpty(name))
                return new Bitmap(144, 144);

            Stream manifestResourceStream = GetType().Assembly.GetManifestResourceStream($"CodeRushStreamDeck.images.resources.{name}@2x.png");
            
            if (manifestResourceStream == null)
                return new Bitmap(144, 144);

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
