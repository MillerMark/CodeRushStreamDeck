﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public class TextLine
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// The width of the text.
        /// </summary>
        public int Width { get; set; }
        public TextLine()
        {

        }
        public void SetWidth(float width)
        {
            Width = (int)Math.Ceiling(width);
        }
        
        public void Draw(Graphics graphics, Font font, int counter)
        {
            int xOffset = 0;
            if (Width > ButtonText.ButtonWidth)
            {
                int pixelsToMoveOneWay = Width - ButtonText.ButtonWidth;
                int totalPixelsToMove = pixelsToMoveOneWay * 2;
                int pixelsIntoThisMove = counter % totalPixelsToMove;
                if (pixelsIntoThisMove < pixelsToMoveOneWay)
                {
                    xOffset = -pixelsIntoThisMove;
                }
                else
                {
                    xOffset = pixelsIntoThisMove - 2 * pixelsToMoveOneWay;
                }

            }
            graphics.DrawString(Text, font, Brushes.White, X + xOffset, Y);
        }
    }
}
