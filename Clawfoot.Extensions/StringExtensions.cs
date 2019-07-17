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
    }
}
