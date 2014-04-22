using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blink.Tests
{
    public static class RegexAssert
    {
        public static void IsMatch(string pattern, string input, RegexOptions options = RegexOptions.None)
        {
            var rx = new Regex(pattern, options);
            var isMatch = rx.IsMatch(input);

            Assert.IsTrue(isMatch, "Failed to match input '{0}' against pattern '{1}'", input, pattern);
        }
    }
}
