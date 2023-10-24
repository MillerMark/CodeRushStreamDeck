using CodeRushStreamDeck.Models;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;
using System.Timers;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public class ScrollingText
    {
        bool needToUpdateDrawingParameters;
        public event EventHandler RefreshImage;
        int counter;
        Timer marqueeTimer = new Timer(50);

        const string STR_FontName = "Arial";
        float fontSize = 22;
        public const int ButtonWidth = 144;
        public const int ButtonHeight = 144;

        Font font;
        bool hasOverflow;

        public List<TextLine> TextLines { get; set; } = new List<TextLine>();
        public Font Font { get; set; }
        public Brush Brush { get; set; }

        public bool HasOverflow => hasOverflow;

        float GetFontSize(Graphics background, ICanAddTextLines canAddTextLines)
        {
            const float minFontSize = 22f;
            float fontSize = 20;
            switch (canAddTextLines.LineCount)
            {
                case 1:
                    fontSize = 45;
                    Font testFont = GetFont(fontSize);
                    string typeName = canAddTextLines.GetSimpleText();
                    SizeF measureString = background.MeasureString(typeName, testFont);
                    while (measureString.Width > ButtonWidth)
                    {
                        fontSize--;
                        if (fontSize <= minFontSize)
                            break;
                        testFont = GetFont(fontSize);
                        measureString = background.MeasureString(typeName, testFont);
                    }
                    break;
                case 2:
                    fontSize = 30;
                    break;
                case 3:
                    fontSize = 22;
                    break;
            }
            return fontSize;
        }

        private static Font GetFont(float fontSize)
        {
            return new Font(STR_FontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public void Draw(Graphics graphics)
        {
            CheckOverflow();
            foreach (TextLine textLine in TextLines)
                textLine.Draw(graphics, font, counter);
        }

        public ScrollingText(Graphics graphics, ICanAddTextLines canAddTextLines, EventHandler refreshImageEventHandler)
        {
            marqueeTimer.Elapsed += MarqueeTimer_Elapsed;
            UpdateDrawingParameters(graphics, canAddTextLines);
            RefreshImage += refreshImageEventHandler;
        }

        private void MarqueeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            counter++;
            RefreshImage?.Invoke(sender, e);
        }

        public void UpdateDrawingParameters(Graphics graphics, ICanAddTextLines canAddTextLines)
        {
            fontSize = GetFontSize(graphics, canAddTextLines);
            // Max font size for three lines: 22
            // Max font size for two lines: 30
            // Max font size for one line: 45
            // Max font size for readability: 22?


            float lineHeight = fontSize * 1.2f;
            float x = 0;
            float line1 = ButtonHeight - lineHeight * 3;
            float line2 = ButtonHeight - lineHeight * 2;
            float line3 = ButtonHeight - lineHeight;
            font = new Font(STR_FontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            TextLines.Clear();
            canAddTextLines.AddTextLines(TextLines, x, line1, line2, line3);

            CheckTextLinesForOverflow(graphics);
        }

        void CheckTextLinesForOverflow(Graphics graphics)
        {
            hasOverflow = false;
            foreach (TextLine textLine in TextLines)
            {
                textLine.SetWidth(graphics.MeasureString(textLine.Text, font).Width);
                if (textLine.Width > ButtonWidth)
                    hasOverflow = true;
            }
        }

        public void CheckOverflow()
        {
            if (HasOverflow)
            {
                if (!marqueeTimer.Enabled)
                {
                    counter = 0;
                    marqueeTimer.Start();
                }
            }
            else if (marqueeTimer.Enabled)
                marqueeTimer.Stop();
        }

        public void CheckDrawingParameters(Graphics background, ICanAddTextLines settingsModel)
        {
            if (!needToUpdateDrawingParameters)
                return;

            needToUpdateDrawingParameters = false;
            UpdateDrawingParameters(background, settingsModel);
        }

        public void InvalidateDrawingParameters()
        {
            needToUpdateDrawingParameters = true;
        }
    }
}
