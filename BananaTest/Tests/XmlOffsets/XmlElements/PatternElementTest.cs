using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using BananaPattern;
using BananaPattern.Algorithms;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class PatternElementTest : XElementWrapperTestBase
    {
        [TestMethod]
        public void DefaultContructor_Properties_AreEmpty()
        {
            PatternElement patternEle = new PatternElement();

            Assert.AreEqual("", patternEle.Name);
            Assert.AreEqual(0, patternEle.Pattern._pattern.Length);
            Assert.IsNull(patternEle.Cached);
        }

        [TestMethod]
        public void Name_Get_ReadsNameElement()
        {
            string expected = "Hi!";
            XElement element = CreatePatternXElement(expected);
            PatternElement patternEle = new PatternElement(element);

            Assert.AreEqual(expected, patternEle.Name);
        }

        [TestMethod]
        public void Name_Set_SetsNameElement()
        {
            string expected = "Hi!";
            XElement element = CreatePatternXElement("some other name");
            PatternElement patternEle = new PatternElement(element);
            patternEle.Name = expected;

            string actual = element.Element(element.GetDefaultNamespace() + "Name").Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Pattern_Get_ReadsPatternElement()
        {
            XElement element = CreatePatternXElement("some name", "AA BB CC");
            PatternElement patternEle = new PatternElement(element);

            CollectionAssert.AreEqual(new byte[] { 0xAA, 0xBB, 0xCC }, patternEle.Pattern._pattern);
        }

        [TestMethod]
        public void Pattern_Set_SetsPatternElement()
        {
            string expected = "AA ? BB";
            XElement element = CreatePatternXElement("some name");
            PatternElement patternEle = new PatternElement(element);
            patternEle.Pattern = Pattern.FromCombinedString(expected);

            string actual = element.Element(element.GetDefaultNamespace() + "Pattern").Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Cached_Get_ReadsCachedElement()
        {
            XElement cached = CreateCachedXElement("blub", "value");
            XElement element = CreatePatternXElement("some name", "AA BB CC", cached);
            PatternElement patternEle = new PatternElement(element);

            var cachedEle = patternEle.Cached;

            Assert.AreEqual(cached, cachedEle.Element);
        }

        [TestMethod]
        public void Cached_Set_SetsCachedElement()
        {
            string expectedBuild = "1.1.1";
            int expectedValue = 12;
            XElement element = CreatePatternXElement("some name", "AA BB CC");
            PatternElement patternEle = new PatternElement
            {
                Cached = new CachedElement { Build = expectedBuild, Value = expectedValue }
            };

            var cachedEle = patternEle.Cached;

            Assert.AreEqual(expectedBuild, cachedEle.Build);
            Assert.AreEqual(expectedValue, cachedEle.Value);
        }

        [TestMethod]
        public void Cached_Set_ReplacesCachedElement()
        {
            string expectedBuild = "1.1.1";
            int expectedValue = 12;
            XElement cached = CreateCachedXElement("1.1.0", "45");
            XElement element = CreatePatternXElement("some name", "AA BB CC", cached);
            PatternElement patternEle = new PatternElement(element);
            patternEle.Cached = new CachedElement { Build = expectedBuild, Value = expectedValue };

            var cachedEle = patternEle.Cached;

            Assert.AreEqual(expectedBuild, cachedEle.Build);
            Assert.AreEqual(expectedValue, cachedEle.Value);
            Assert.IsNull(cached.Parent);
        }

        [TestMethod]
        public void Cached_SetNull_RemovesCachedElement()
        {
            XElement cached = CreateCachedXElement("some", "value");
            XElement element = CreatePatternXElement("some name", "AA BB CC", cached);
            PatternElement patternEle = new PatternElement(element);

            patternEle.Cached = null;

            Assert.IsFalse(element.Elements("Cached").Any());
        }

        [TestMethod]
        public void Find_ValidContext_ForwardsToPattern()
        {
            IntPtr expected = new IntPtr(0x1337);
            TestPatternElement element = new TestPatternElement();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            patternMock.Setup(x => x.Find(null, null)).Returns(expected);
            element.Pattern = patternMock.Object;

            IntPtr actual = element.Find(null);

            Assert.AreEqual(actual, expected);
            patternMock.Verify(x => x.Find(null, null));
        }

        [TestMethod]
        public void FindCached_NotCached_ForwardsToPattern()
        {
            IntPtr expected = new IntPtr(0x1337);
            TestPatternElement element = new TestPatternElement();
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            patternMock.Setup(x => x.Find(contextMock.Object, null)).Returns(expected);
            element.Pattern = patternMock.Object;

            IntPtr actual = element.FindCached(contextMock.Object);

            Assert.AreEqual(expected, actual);
            patternMock.Verify(x => x.Find(contextMock.Object, null));
        }

        [TestMethod]
        public void FindCached_NotCached_CachesResult()
        {
            IntPtr baseAddress = Process.GetCurrentProcess().MainModule.BaseAddress;
            IntPtr expected = baseAddress + 0x1337;
            TestPatternElement element = new TestPatternElement();
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            patternMock.Setup(x => x.Find(contextMock.Object, null)).Returns(expected);
            element.Pattern = patternMock.Object;

            element.FindCached(contextMock.Object);
            IntPtr actual = baseAddress + element.Cached.Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindCached_CachedOutdated_ForwardsToPattern()
        {
            IntPtr expected = new IntPtr(0x1337);
            TestPatternElement element = new TestPatternElement
            {
                Cached = new CachedElement { Value = 4, Build = "0.0.0" }
            };
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            patternMock.Setup(x => x.Find(contextMock.Object, null)).Returns(expected);
            element.Pattern = patternMock.Object;

            IntPtr actual = element.FindCached(contextMock.Object);

            Assert.AreEqual(expected, actual);
            patternMock.Verify(x => x.Find(contextMock.Object, null));
        }

        [TestMethod]
        public void FindCached_CachedOutdated_CachesResult()
        {
            IntPtr baseAddress = Process.GetCurrentProcess().MainModule.BaseAddress;
            IntPtr expected = baseAddress + 0x1337;
            TestPatternElement element = new TestPatternElement
            {
                Cached = new CachedElement { Value = 4, Build = "0.0.0" }
            };
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            patternMock.Setup(x => x.Find(contextMock.Object, null)).Returns(expected);
            element.Pattern = patternMock.Object;

            element.FindCached(contextMock.Object);
            IntPtr actual = baseAddress + element.Cached.Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FindCached_CachedUpToDate_ReturnsCachedValue()
        {
            var info = Process.GetCurrentProcess().MainModule.FileVersionInfo;
            int expected = 0x1337;
            TestPatternElement element = new TestPatternElement
            {
                Cached = new CachedElement { Value = expected, Build = info.FileVersion }
            };
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            element.Pattern = patternMock.Object;

            element.FindCached(contextMock.Object);
            int actual = element.Cached.Value;

            Assert.AreEqual(expected, actual);
            patternMock.Verify(x => x.Find(contextMock.Object, null), Times.Never());
        }

        [TestMethod]
        public void FindCached_CachedUpToDate_ReturnsSameValueAsNotCached()
        {
            var info = Process.GetCurrentProcess().MainModule.FileVersionInfo;
            IntPtr expected = Process.GetCurrentProcess().MainModule.BaseAddress + 0x1337;
            TestPatternElement element = new TestPatternElement();

            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            Mock<Pattern> patternMock = new Mock<Pattern>(new byte[0], (bool[])null, false);
            patternMock.Setup(x => x.Find(contextMock.Object, null)).Returns(expected);
            element.Pattern = patternMock.Object;

            IntPtr actual1 = element.FindCached(contextMock.Object);
            IntPtr actual2 = element.FindCached(contextMock.Object);

            Assert.AreEqual(expected, actual1);
            Assert.AreEqual(expected, actual2);
            patternMock.Verify(x => x.Find(contextMock.Object, null), Times.Once());
        }

        private class TestPatternElement : PatternElement
        {
            public override Pattern Pattern { get; set; }
        }
    }
}
