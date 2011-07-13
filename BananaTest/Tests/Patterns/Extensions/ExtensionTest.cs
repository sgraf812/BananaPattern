using System;
using BananaPattern.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.Patterns.Helper
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void FindFirst_ReturnsFirstMatch()
        {
            int expected = 2;
            bool[] sequence = { false, false, true, true, false };

            int actual = sequence.FindFirst(b => b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FindFirst_NoMatch_Throws()
        {
            bool[] sequence = { false, false };

            int actual = sequence.FindFirst(b => b);
        }

        [TestMethod]
        public void FindLast_ReturnsFirstMatch()
        {
            int expected = 3;
            bool[] sequence = { false, false, true, true, false };

            int actual = sequence.FindLast(b => b);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FindLast_NoMatch_Throws()
        {
            bool[] sequence = { false, false };

            int actual = sequence.FindLast(b => b);
        }
    }
}
