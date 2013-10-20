/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

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