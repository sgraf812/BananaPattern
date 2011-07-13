using System;
using BananaPattern.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.Patterns
{
    [TestClass]
    public abstract unsafe class AlgorithmTest
    {
        private readonly byte[] _bytePattern = new byte[] { 0x10, 0x20, 0x00, 0x40, 0x00 };

        private readonly bool[] _mask = new bool[] { true, true, false, true, false };

        protected IPatternAlgorithm _algorithm;

        protected abstract IPatternAlgorithm CreateAlgorithm();

        [TestInitialize]
        public void Setup()
        {
            _algorithm = CreateAlgorithm();
        }

        [TestMethod]
        public void Apply_NoMatch_ReturnsZeroPointer()
        {
            var range = new byte[] { 0x42, 0x55, 0x23, 0x45, 0x42 }; // no match

            fixed (byte* begin = range)
            {
                IntPtr actual = _algorithm.Apply(_bytePattern, _mask, begin, begin + range.Length);

                Assert.AreEqual(IntPtr.Zero, actual);
            }
        }

        [TestMethod]
        public void Apply_MatchAtBegin_CorrectResult()
        {
            var range = new byte[] { 0x10, 0x20, 0x54, 0x40, 0x87 }; // match

            fixed (byte* begin = range)
            {
                IntPtr actual = _algorithm.Apply(_bytePattern, _mask, begin, begin + range.Length);

                Assert.AreEqual((IntPtr)begin, actual);
            }
        }

        [TestMethod]
        public void Apply_MatchAtEnd_CorrectResult()
        {
            var range = new byte[] 
            { 
                0x65, 0x21 , 0x54, 0x10, 0x20, 0x45,0x45, 0x50, 0x40, 0x10, 
                0x10, 0x20, 0x54, 0x40, 0x87 // match
            };

            fixed (byte* begin = range)
            {
                IntPtr actual = _algorithm.Apply(_bytePattern, _mask, begin, begin + range.Length);
                var end = begin + 10;

                Assert.AreEqual((IntPtr)end, actual);
            }
        }

        [TestMethod]
        public void Apply_ManyMatches_FirstMatchReturned()
        {
            var range = new byte[] 
            {
                0x40, 0x50, 0x54, 0x04, 0x78, 
                0x10, 0x20, 0x54, 0x40, 0x87, // match
                0x54, 0x52, 0x21, 0x77, 0x87,
                0x10, 0x20, 0x54, 0x40, 0x87, // match
                0x10, 0x20, 0x54, 0x40, 0x87, // match
            };

            fixed (byte* begin = range)
            {
                IntPtr actual = _algorithm.Apply(_bytePattern, _mask, begin, begin + range.Length);

                Assert.AreEqual((IntPtr)begin + 5, actual);
            }
        }
    }
}
