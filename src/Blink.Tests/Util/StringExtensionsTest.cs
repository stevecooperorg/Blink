using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blink.Util;

namespace Blink.Tests.Util
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void WithoutRemovesCharacters()
        {
            var original = "abcdef";
            var actual = original.Without("bdf".ToArray());
            var expected = "ace";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WithoutRemovesPathCharacters()
        {
            var original = "abc/\\\":";
            var actual = original.WithoutPathCharacters();
            var expected = "abc";
            Assert.AreEqual(expected, actual);
        }
    }
}
