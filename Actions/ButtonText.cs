using CodeRushStreamDeck.Models;
using DevExpress.CodeRush.Foundation.Pipes.Data;
using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Versioning;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    public class ButtonText
    {
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

        float GetFontSize(Graphics background, SpokenTypeTemplateData spokenTypeTemplateData)
        {
            const float minFontSize = 22f;
            float fontSize = 20;
            switch (spokenTypeTemplateData.Kind)
            {
                case TypeKind.Simple:
                    fontSize = 45;
                    Font testFont = GetFont(fontSize);
                    string typeName = GetOnlyTypeName(spokenTypeTemplateData.SimpleType);
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
                case TypeKind.GenericOneTypeParameter:
                    fontSize = 30;
                    break;
                case TypeKind.GenericTwoTypeParameters:
                    fontSize = 22;
                    break;
            }
            return fontSize;
        }

        private static Font GetFont(float fontSize)
        {
            return new Font(STR_FontName, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        string GetOnlyTypeName(string typeName)
        {
            int lastIndexOfDot = typeName.LastIndexOf('.');
            if (lastIndexOfDot < typeName.Length - 1)
                return typeName.Substring(lastIndexOfDot + 1);
            return typeName;
        }

        public void Draw(Graphics graphics, int counter)
        {
            foreach (TextLine textLine in TextLines)
                textLine.Draw(graphics, font, counter);
        }

        public ButtonText(Graphics graphics, SpokenTypeTemplateData spokenTypeTemplateData)
        {
            UpdateDrawParameters(graphics, spokenTypeTemplateData);
        }

        public void UpdateDrawParameters(Graphics graphics, SpokenTypeTemplateData spokenTypeTemplateData)
        {
            fontSize = GetFontSize(graphics, spokenTypeTemplateData);
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
            switch (spokenTypeTemplateData.Kind)
            {
                case TypeKind.Simple:
                    TextLines.Add(new TextLine() { Text= GetOnlyTypeName(spokenTypeTemplateData.SimpleType), X = x, Y = line3 });
                    break;
                case TypeKind.GenericOneTypeParameter:
                    TextLines.Add(new TextLine() { Text = GetOnlyTypeName(spokenTypeTemplateData.GenericType), X = x, Y = line2 });
                    TextLines.Add(new TextLine() { Text = $"<{GetOnlyTypeName(spokenTypeTemplateData.TypeParam1)}>", X = x, Y = line3 });
                    break;
                case TypeKind.GenericTwoTypeParameters:
                    TextLines.Add(new TextLine() { Text = GetOnlyTypeName(spokenTypeTemplateData.GenericType), X = x, Y = line1 });
                    TextLines.Add(new TextLine() { Text = $"<{GetOnlyTypeName(spokenTypeTemplateData.TypeParam1)}, ", X = x, Y = line2 });
                    TextLines.Add(new TextLine() { Text = $"{GetOnlyTypeName(spokenTypeTemplateData.TypeParam2)}>", X = x, Y = line3 });
                    break;
            }

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
    }
}
