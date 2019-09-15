using Clawfoot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Clawfoot.Extensions
{
    public static class StringExtensions
    {
        //From GenericServices
        /// <summary>
        /// This splits up a string based on capital letters
        /// e.g. "MyAction" would become "My Action" and "My10Action" would become "My10 Action"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SplitPascalCase(this string str)
        {
            Regex regex = new Regex("([a-z,0-9](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", RegexOptions.Compiled);
            return regex.Replace(str, "$1 ");
        }

        /// <summary>
        /// Returns a new string that center-aligns the characters in this instance by padding them on the left and the right with a specified Unicode character, for a specified total length.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength">The number of characters in the resulting string, equal to the number of original characters plus any additional padding characters.</param>
        /// <param name="paddingCharacter">A Unicode padding character.</param>
        /// <param name="paddingStartSide">The side that padding will start at, determines which side will have an additional character if the string has an odd length</param>
        /// <param name="padIfNPlusOne">If the string should be padded if it's length == maxLength+1</param>
        /// <returns>
        ///     A new string that is equivalent to this instance, but center-aligned and padded on the left and right 
        ///     with as many paddingChar characters as needed to create a length of totalWidth. 
        ///     If totalWidth is equal to or less than the length of this instance, the method returns a reference to the existing instance.
        ///     If totalWidth is equal to the length of this instance + 1, it will pad only to the paddingStartSide, if padIfNPlusOne is true. Otherwise it will return a reference to the existing instance.
        ///</returns>
        public static string PadLeftAndRight(this string str, int maxLength, char paddingCharacter, RelativeHandedSide paddingStartSide = RelativeHandedSide.Left, bool padIfNPlusOne = true)
        {
            if (str.Length >= maxLength)
            {
                return str;
            }

            if (str.Length  == maxLength + 1 && !padIfNPlusOne)
            {
                return str;
            }

            int padPerSide = (int)Math.Floor( ((double)maxLength - (double)str.Length) / 2d);
            int carryover = maxLength - (padPerSide * 2) - str.Length;
            int leftSideTotalLength = str.Length + padPerSide; // Total length after first side is padded (without carryover)

            // Only need the left as that's the first pad. 
            // If there is no carryover on the left, it will be automatically applied on the right thanks to maxLength
            leftSideTotalLength += paddingStartSide == RelativeHandedSide.Left ? carryover : 0;

            string paddedLeft = str.PadLeft(leftSideTotalLength, paddingCharacter);
            return paddedLeft.PadRight(maxLength, paddingCharacter);
        }

        /// <summary>
        /// Performant, single-pass, string wrapper that will
        /// wrap the string based on the maxWidth while attempting to keep words intact
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        public static string Wrap(this string text, int maxWidth)
        {
            StringBuilder builder = new StringBuilder();

            int lastsplit = 0;
            int lastWhiteSpace = 0;
            bool lastSplitOnSpace = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]))
                {
                    if (!(i - lastsplit < maxWidth && i < text.Length))
                    {
                        if (i - lastsplit == maxWidth)
                        {
                            if (builder.Length == 0)
                            {
                                builder.AppendLine(text.Substring(lastsplit, i - lastsplit));
                            }
                            else
                            {
                                builder.AppendLine(text.Substring(lastsplit + 1, i - lastsplit - 1));
                            }

                            lastsplit = i;
                            lastWhiteSpace = i;
                            lastSplitOnSpace = true;
                        }
                        //Current length is over limit, new whitespace found, size of next split area is less than limit, then split on last found white space
                        else if (i - lastsplit > maxWidth && lastsplit != lastWhiteSpace && lastWhiteSpace - lastsplit - 1 <= maxWidth)
                        {
                            if (builder.Length == 0)
                            {
                                builder.AppendLine(text.Substring(lastsplit, lastWhiteSpace - lastsplit));
                            }
                            else
                            {
                                builder.AppendLine(text.Substring(lastsplit + 1, lastWhiteSpace - lastsplit - 1));
                            }
                            lastsplit = lastWhiteSpace; //Split was performed at the last whitespace
                            lastWhiteSpace = i; //On a new whitespace right now, set that accordingly
                            lastSplitOnSpace = true;
                        }
                        //Last whitespace and last split are in the same location, and text is longer than limit. Means single word is longer than limit, then split inside word at limit
                        else
                        {
                            if (Char.IsWhiteSpace(text[lastsplit])) //Last split was a whitespace, skip forward 1 char to skip whitespace
                            {
                                builder.AppendLine(text.Substring(lastsplit + 1, maxWidth));
                                lastsplit += maxWidth + 1;
                            }
                            else
                            {
                                builder.AppendLine(text.Substring(lastsplit, maxWidth));
                                lastsplit += maxWidth;
                            }
                            lastWhiteSpace = i; //On a new whitespace right now, set that accordingly
                            lastSplitOnSpace = false;
                            continue;
                        }
                    }
                    else
                    {
                        lastWhiteSpace = i;
                    }

                    if (i + 1 != text.Length && Char.IsWhiteSpace(text[i + 1])) //If next char is whitespace, move forward till no more white space
                    {
                        i++;
                        for (; i < text.Length; i++)
                        {
                            if (Char.IsWhiteSpace(text[i]))
                            {
                                continue;
                            }
                            else
                            {
                                i--; //Current character isn't whitespace, go back a character
                                lastWhiteSpace = i;
                                lastsplit = i;
                                break;
                            }
                        }
                    }
                }

                if (i + 1 == text.Length)
                {
                    if (lastSplitOnSpace) //split was done on a space, skip forward one to skip excess space
                    {
                        builder.AppendLine(text.Substring(lastsplit + 1, i - lastsplit));
                    }
                    else //Split wasn't done on a space
                    {
                        builder.AppendLine(text.Substring(lastsplit, i - lastsplit + 1));
                    }
                }
            }
            return builder.ToString();
        }
    }
}
