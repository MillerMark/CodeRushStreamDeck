using System;
using System.Collections.Generic;
using DevExpress.CodeRush.Foundation.Pipes.Data;

namespace CodeRushStreamDeck.Models
{
    public interface ICanAddTextLines
    {
        /// <summary>
        /// Add text lines as necessary. Implementors will add 1, 2, or 3 lines of text. line1 is the top line. line3 is 
        /// the bottom line (at the bottom of the button). If only one line is added, use line3 as the y position. If 
        /// only two lines are added, use line2 and line3. 
        /// </summary>
        /// <param name="textLines">The List of TextLines to add to.</param>
        /// <param name="x">The x position for each line.</param>
        /// <param name="line1">The y position of the top line.</param>
        /// <param name="line2">The y position of the second line.</param>
        /// <param name="line3">The y position of the bottom line.</param>
        void AddTextLines(List<TextLine> textLines, float x, float line1, float line2, float line3);

        /// <summary>
        /// Returns the text for scenarios where there is only one line.
        /// </summary>
        /// <returns></returns>
        string GetSimpleText();

        /// <summary>
        /// Returns the number of lines (1, 2, or 3) to show in the Stream Deck.
        /// </summary>
        int LineCount { get; }
    }
}
