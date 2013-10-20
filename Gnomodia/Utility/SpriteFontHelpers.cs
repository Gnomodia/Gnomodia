using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Gnomodia.Utility
{
    public static class SpriteFontHelpers
    {
        public static string BreakStringAccoringToLineLength(SpriteFont font, string input, float maxLineLength)
        {
            var words = input.Split(' ');
            var lines = new List<string>();
            var spaceLen = font.MeasureString(" ").X;
            var currentLineLength = 0f;
            StringBuilder currentLine = new StringBuilder();
            foreach (var word in words)
            {
                var wordLen = font.MeasureString(word).X;
                if (currentLineLength > 0)
                {
                    if ((currentLineLength + spaceLen + wordLen) > maxLineLength)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        currentLine.Append(word);
                        currentLineLength = wordLen;
                    }
                    else
                    {
                        currentLine.Append(' ').Append(word);
                        currentLineLength += wordLen + spaceLen;
                    }
                }
                else
                {
                    currentLine.Append(word);
                    currentLineLength += wordLen;
                }
            }
            lines.Add(currentLine.ToString());
            return string.Join("\n", lines);
        }

        private static string AddSpacesAccordingToLength(string text, float spaceLength, float remainingLength)
        {
            var cntFloat = (int)Math.Floor(remainingLength / spaceLength);
            if( cntFloat <= 0 )
                return text;
            var both = cntFloat >> 1;
            return new string(' ', both + (cntFloat % 2)) + text + new string(' ', both);
        }

        public static string BreakAndCenterStringAccoringToLineLength(SpriteFont font, string input, float maxLineLength)
        {
            var words = input.Split(' ');
            var lines = new List<string>();
            var spaceLen = font.MeasureString(" ").X;
            var currentLineLength = 0f;
            StringBuilder currentLine = new StringBuilder();
            foreach (var word in words)
            {
                var wordLen = font.MeasureString(word).X;
                if (currentLineLength > 0)
                {
                    if ((currentLineLength + spaceLen + wordLen) > maxLineLength)
                    {
                        lines.Add(AddSpacesAccordingToLength(currentLine.ToString(), spaceLen, maxLineLength - currentLineLength));
                        currentLine.Clear();
                        currentLine.Append(word);
                        currentLineLength = wordLen;
                    }
                    else
                    {
                        currentLine.Append(' ').Append(word);
                        currentLineLength += wordLen + spaceLen;
                    }
                }
                else
                {
                    currentLine.Append(word);
                    currentLineLength += wordLen;
                }
            }
            lines.Add(AddSpacesAccordingToLength(currentLine.ToString(), spaceLen, maxLineLength - currentLineLength));
            return string.Join("\n", lines);
        }
    }
}