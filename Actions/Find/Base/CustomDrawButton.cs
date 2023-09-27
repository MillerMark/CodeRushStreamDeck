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
        object backgroundImageLock = new object();
        protected Bitmap backgroundImage;
        protected abstract string BackgroundImageName { get; }
        
        protected async Task DrawBackgroundAsync()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                lock (backgroundImageLock)
                    backgroundImage.Save(stream, ImageFormat.Png); 

                byte[] imageBytes = stream.ToArray();
                string imageLocation = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                await Manager.SetImageAsync(lastContext, imageLocation);
            }
        }

        protected virtual Graphics GetBackground()
        {
            lock (backgroundImageLock)
            {
                backgroundImage = GetBitmapResource(BackgroundImageName);
                return Graphics.FromImage(backgroundImage);
            }
        }

        protected Bitmap GetBitmapResource(string name)
        {
            Stream manifestResourceStream = GetType().Assembly.GetManifestResourceStream($"CodeRushStreamDeck.images.resources.{name}@2x.png");
            
            if (manifestResourceStream == null)
                return new Bitmap(144, 144);

            return new Bitmap(manifestResourceStream);
        }

        public override async Task OnWillAppear(StreamDeckEventPayload args)
        {
            await base.OnWillAppear(args);

            using (Graphics background = GetBackground())
            {
                // Do nothing - call to GetBackground is all that is needed to get the correct image.
            }
            await DrawBackgroundAsync();
        }
    }
}
