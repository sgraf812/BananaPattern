using System;
using System.Collections.Generic;
using BananaPattern;
using BananaPattern.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BananaTest.Tests.Patterns
{
    [TestClass]
    public class PatternTest
    {
        private readonly byte[] _bytePattern =
            new byte[] { 0x00, 0x00, 0x00, 0x10, 0x20, 0x30, 0x40, 0x00, 0x00 };

        private readonly bool[] _mask =
            new bool[] { false, false, false, true, true, true, true, true, false };

        private const string _stringPattern = "? ? ? 10 20 ? 40 00 ?";

        private Mock<IBotProcessContext> SetupContextMock()
        {
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            contextMock.Setup(x => x.IsInProcess).Returns(true);

            return contextMock;
        }

        [TestCleanup]
        public void Cleanup()
        {
            Pattern.ResetInternals();
        }

        [TestMethod]
        public void Constructor_TrimWildCardsTrue_MaskIsTrimmed()
        {
            Pattern pattern = new Pattern(_bytePattern, _mask, true);
            var expected = new bool[] { true, true, true, true, true };

            CollectionAssert.AreEquivalent(expected, pattern._mask);
        }

        [TestMethod]
        public void Constructor_TrimWildCardsTrue_PatternIsTrimmed()
        {
            Pattern pattern = new Pattern(_bytePattern, _mask, true);
            var expected = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x00 };

            CollectionAssert.AreEquivalent(expected, pattern._pattern);
        }


        [TestMethod]
        public void FromCombinedString_ValidCall_ReturnsTrimmedPattern()
        {
            Pattern pattern = Pattern.FromCombinedString(_stringPattern);

            var expected = new byte[] { 0x10, 0x20, 0x00, 0x40, 0x00 };

            CollectionAssert.AreEquivalent(expected, pattern._pattern);
        }

        [TestMethod]
        public void ToCombinedString_HexDigits_AreUpperCase()
        {
            string expected = "aa";

            Pattern pattern = Pattern.FromCombinedString(expected);

            Assert.AreEqual(expected.ToUpper(), pattern.ToCombinedString());
        }

        [TestMethod]
        public void ToCombinedString_OutputString_HasNoPadding()
        {
            string expected = "AA";

            Pattern pattern = Pattern.FromCombinedString(expected);
            string actual = pattern.ToCombinedString();
            bool hasPadding = actual.StartsWith(" ") || actual.EndsWith(" ");

            Assert.IsFalse(hasPadding);
        }

        [TestMethod]
        public void ToCombinedString_OutputString_MatchesInputString()
        {
            string expected = "AA ? BB ? CC ? DD";

            Pattern pattern = Pattern.FromCombinedString(expected);

            Assert.AreEqual(expected, pattern.ToCombinedString());
        }

        [TestMethod]
        public unsafe void FindMany_MaxCountEquals4_4MatchesAtMax()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            Pattern pattern = Pattern.FromCombinedString("24 25");
            byte[] region = new byte[]
                { 0x24, 0x25, 0x00, 0x24, 0x25, 0x00, 0x24, 0x25, 0x00, 0x24, 0x25, 0x00,
                    0x24, 0x25, 0x00};

            IList<IntPtr> matches;
            fixed (byte* r = region)
            {
                matches = pattern.FindMany((IntPtr)r, (IntPtr)r + region.Length, contextMock.Object, 4);
            }

            Assert.IsTrue(matches.Count <= 4);
        }

        [TestMethod]
        [ExpectedException(typeof(PatternException))]
        public unsafe void Find_NoMatch_Throws()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            Pattern pattern = Pattern.FromCombinedString("24 25");
            byte[] region = new byte[] { 0x00 };

            IntPtr match;
            fixed (byte* r = region)
            {
                match = pattern.Find((IntPtr)r, (IntPtr)r + region.Length, contextMock.Object);
            }
        }

        [TestMethod]
        public unsafe void FindMany_EmptySearchRange_ReturnsNoMatch()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            Pattern pattern = Pattern.FromCombinedString("24 25");
            byte[] region = new byte[] { };

            IList<IntPtr> matches;
            fixed (byte* r = region)
            {
                matches = pattern.FindMany((IntPtr)r, (IntPtr)r + region.Length, contextMock.Object, 4);
            }

            Assert.IsTrue(matches.Count == 0);
        }

        [TestMethod]
        public void Find_InContextModule_Succeeds()
        {

            Mock<IBotProcessContext> contextMock = SetupContextMock();

            // 0x00 will be somewhere in memory for sure.
            Pattern pattern = Pattern.FromCombinedString("00");

            var actual = pattern.Find(contextMock.Object);

            Assert.AreNotEqual(IntPtr.Zero, actual);
        }

        [TestMethod]
        public void Find_InProcessModule_Succeeds()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            // 0x00 will be somewhere in memory for sure.
            Pattern pattern = Pattern.FromCombinedString("00");

            var actual = pattern.Find(contextMock.Object.TargetProcess.MainModule, contextMock.Object);

            Assert.AreNotEqual(IntPtr.Zero, actual);
        }

        [TestMethod]
        public void FindMany_InContextModule_Succeeds()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            // 0x00 will be somewhere in memory for sure.
            Pattern pattern = Pattern.FromCombinedString("00");

            var actual = pattern.FindMany(contextMock.Object, 5);

            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod]
        public void FindMany_InProcessModule_Succeeds()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            // 0x00 will be somewhere in memory for sure.
            Pattern pattern = Pattern.FromCombinedString("00");

            var actual = pattern.FindMany(contextMock.Object.TargetProcess.MainModule, contextMock.Object, 5);

            Assert.IsTrue(actual.Count > 0);
        }

        [TestMethod]
        public unsafe void Find_AlgorithmIsNull_UsesDefaultAlgorithm()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            Pattern pattern = Pattern.FromCombinedString("24 25");
            StubSaveRangeAlgorithm stubAlgorithm = new StubSaveRangeAlgorithm();
            byte[] region = new byte[]
                { 0x24, 0x25, 0x00, 0x24, 0x25, 0x00, 0x24, 0x25, 0x00, 0x24, 0x25, 0x00,
                    0x24, 0x25, 0x00};
            Pattern.DefaultAlgorithm = stubAlgorithm;

            fixed (byte* r = region)
            {
                IntPtr begin = (IntPtr)r;
                IntPtr end = (IntPtr)r + region.Length;

                IList<IntPtr> matches = pattern.FindMany(begin, end, contextMock.Object, 1);

                Assert.AreEqual(begin, stubAlgorithm.Min);
                Assert.AreEqual(end, stubAlgorithm.Max);
            }
        }

        [TestMethod]
        public unsafe void Find_AlgorithmIsCalled_ForWholeRange()
        {
            Mock<IBotProcessContext> contextMock = SetupContextMock();

            Pattern pattern = Pattern.FromCombinedString("24 25");
            StubSaveRangeAlgorithm stubAlgorithm = new StubSaveRangeAlgorithm();
            byte[] region = new byte[]
                { 0x24, 0x25, 0x00, 0x24, 0x25, 0x00, 0x24, 0x25, 0x00, 0x24, 0x25, 0x00,
                    0x24, 0x25, 0x00};

            fixed (byte* r = region)
            {
                IntPtr begin = (IntPtr)r;
                IntPtr end = (IntPtr)r + region.Length;

                IList<IntPtr> matches = pattern.FindMany(begin, end, contextMock.Object, 1, stubAlgorithm);

                Assert.AreEqual(begin, stubAlgorithm.Min);
                Assert.AreEqual(end, stubAlgorithm.Max);
            }
        }

        private unsafe class StubSaveRangeAlgorithm : IPatternAlgorithm
        {
            public IntPtr Min { get; private set; }

            public IntPtr Max { get; private set; }

            public StubSaveRangeAlgorithm()
            {
                Min = new IntPtr(IntPtr.Size == 4 ? int.MaxValue : long.MaxValue);
                Max = IntPtr.Zero;
            }

            #region IPatternAlgorithm Member

            public unsafe IntPtr Apply(byte[] pattern, bool[] mask, byte* begin, byte* end)
            {
                Min = begin < (byte*)Min ? (IntPtr)begin : Min;
                Max = end > (byte*)Max ? (IntPtr)end : Max;

                return IntPtr.Zero;
            }

            #endregion
        }
    }
}
