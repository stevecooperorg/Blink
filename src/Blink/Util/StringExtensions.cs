using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blink.Util
{
    public static class StringExtensions
    {
        public static string Without(this string str, char[] chars)
        {
            foreach (var c in chars)
            {
                str = str.Replace(c.ToString(), string.Empty);
            }

            return str;
        }

        public static string WithoutPathCharacters(this string str)
        {
            return str
                .Without(Path.GetInvalidFileNameChars())
                .Without(Path.GetInvalidPathChars());
        }
    }
}
