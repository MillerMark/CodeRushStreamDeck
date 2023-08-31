using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRushStreamDeck
{
    internal class CamelCaseParser
    {
        /// <summary>
        /// Returns true if the given string is composed entirely of digits.
        /// </summary>
        /// <param name="str">The string to check.</param>
        public static bool IsNumberStr(string str)
        {
            foreach (char chr in str)
                if (!char.IsDigit(chr))
                    return false;
            return str.Length > 0;
        }

        public static List<string> GetWordParts(string text)
        {
            if (text == null)
                return null;
            var parts = new List<string>();
            if (string.IsNullOrEmpty(text))
                return parts;
            var startIndex = 0;
            var i = 0;
            bool breakOnUpperCharacters = text.Any(char.IsLower);
            bool collectingDigit = false;
            bool lastPartAddedWasNumber = false;
            while (i < text.Length)
            {
                var chr = text[i];
                bool isDigit = char.IsDigit(chr);
                if ((char.IsUpper(chr) && breakOnUpperCharacters || chr == ' ' || chr == '.' || isDigit || collectingDigit) && i > startIndex)
                {
                    if (collectingDigit && isDigit)
                    {
                        i++;
                        continue;
                    }

                    collectingDigit = isDigit;
                    string partToAdd = text.Substring(startIndex, i - startIndex).Trim();
                    if (!string.IsNullOrEmpty(partToAdd))
                    {
                        bool thisPartIsNumber = IsNumberStr(partToAdd);

                        if (lastPartAddedWasNumber && thisPartIsNumber)
                            parts[parts.Count - 1] += partToAdd;
                        else if (partToAdd != ".")
                            parts.Add(partToAdd);

                        lastPartAddedWasNumber = thisPartIsNumber;
                    }

                    startIndex = i;
                    if (chr == '.')
                        startIndex++;
                }
                i++;
            }
            string lastPartToAdd = text.Substring(startIndex, i - startIndex).Trim();
            bool lastPartIsNumber = lastPartToAdd.Length == 1 && char.IsDigit(lastPartToAdd[0]);

            if (!string.IsNullOrEmpty(lastPartToAdd))
                if (lastPartAddedWasNumber && lastPartIsNumber)
                    parts[parts.Count - 1] += lastPartToAdd;
                else if (lastPartToAdd != ".")
                    parts.Add(lastPartToAdd);

            return parts.Distinct().ToList();
        }
    }
}
