using System;
using System.Linq;
using System.Drawing;
using System.Runtime.Versioning;
using StreamDeckLib;

namespace CodeRushStreamDeck
{
    [SupportedOSPlatform("windows")]
    [ActionUuid(Uuid = "com.devexpress.coderush.grid.template.expand")]
    public class CodeRushGridTemplateExpandAction : CodeRushTemplateExpandAction
    {
        static int gridColumns = 3;
        static int gridRows = 5;
        protected override string BackgroundImageName => null;
        public CodeRushGridTemplateExpandAction()
        {
            Variables.IntVarChanged += Variables_IntVarChanged;
        }

        private async void Variables_IntVarChanged(object sender, VarEventArgs<int> e)
        {
            if (e.Name == "Grid.Columns")
                gridColumns = Math.Max(1, e.Value);
            else if (e.Name == "Grid.Rows")
                gridRows = Math.Max(1, e.Value);
            await UpdateImageAsync();
        }

        protected override void RefreshButtonImage(Graphics background)
        {
            base.RefreshButtonImage(background);
            int horizontalMargin = 2;
            int verticalMargin = 2;
            const int spaceForBottomLine = 30;
            int drawingWidth = ButtonText.ButtonWidth - horizontalMargin * 2;
            int drawingHeight = ButtonText.ButtonHeight - verticalMargin * 2 - spaceForBottomLine;
            
            int top = verticalMargin;
            int left = horizontalMargin;
            int right = left + drawingWidth;
            int bottom = top + drawingHeight;
            float columnWidth = drawingWidth / gridColumns;
            float rowHeight = drawingHeight / gridRows;
            for (int i = 0; i <= gridColumns; i++)
            {
                int x = (int)Math.Round(horizontalMargin + columnWidth * i);
                background.DrawLine(Pens.White, x, top, x, bottom);
            }
            for (int i = 0; i <= gridRows; i++)
            {
                int y = (int)Math.Round(verticalMargin + rowHeight * i);
                background.DrawLine(Pens.White, left, y, right, y);
            }
            float fontSize = 30;
            Font font = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            background.DrawString($"{gridColumns}x{gridRows}", font, Brushes.White, 0, ButtonText.ButtonHeight - fontSize * 1.2f);
        }
    }
}
