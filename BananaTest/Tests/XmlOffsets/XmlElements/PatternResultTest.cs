using System;
using System.Xml.Linq;
using BananaPattern;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class PatternResultTest : XElementWrapperTestBase
    {
        [TestMethod]
        public void DefaultConstructor_Name_IsEmpty()
        {
            PatternResult result = new PatternResult();

            Assert.AreEqual("", result.Name);
        }

        [TestMethod]
        public void Name_Get_ReadsValueElement()
        {
            string expected = "Hi!";
            XElement element = CreatePatternResultXElement(expected);
            PatternResult result = new PatternResult(element);

            Assert.AreEqual(expected, result.Name);
        }

        [TestMethod]
        public void Name_Set_SetsValueElement()
        {
            string expected = "Hi!";
            PatternResult result = new PatternResult
            {
                Name = expected
            };

            string actual = result.Element.Attribute("Name").Value;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PatternElement_Get_ReturnsNamedPatternElement()
        {
            string expected = "Hi!";
            XElement result = CreatePatternResultXElement(expected);
            XElement pattern = CreatePatternXElement(expected);
            XElement root = new XElement("Patterns",
                pattern, 
                result);
            XDocument doc = new XDocument(root);

            PatternResult_Accessor accessor = new PatternResult_Accessor(result);

            Assert.AreEqual(pattern, accessor.PatternElement.Element);
        }

        [TestMethod]
        public void Execute_RegardlessOfContext_CallsFindCachedOnPatternElement()
        {
            string expected = IntPtr.Zero.ToString("X");
            TestPatternResult result = new TestPatternResult();
            Mock<IBotProcessContext> contextMock = new Mock<IBotProcessContext>();
            contextMock.TargetProcessIsCurrent();
            StubPatternElement patternElement = SetupAndCreatePatternMock(result);

            string actual = result.Execute(contextMock.Object);

            Assert.IsTrue(patternElement.WasCalled);
            Assert.AreEqual(expected, actual);
        }

        private static StubPatternElement SetupAndCreatePatternMock(TestPatternResult result)
        {
            StubPatternElement patternElement = new StubPatternElement();
            result.SetPatternElement(patternElement);
            return patternElement;
        }

        private class TestPatternResult : PatternResult
        {
            private PatternElement _patternElement;
            protected override PatternElement PatternElement { get { return _patternElement; } }

            public void SetPatternElement(PatternElement patternElement)
            {
                _patternElement = patternElement;
            }
        }

        private class StubPatternElement : PatternElement
        {
            public IntPtr ReturnValue { get; set; }
            public bool WasCalled { get; set; }

            public override IntPtr FindCached(IBotProcessContext context)
            {
                WasCalled = true;
                return ReturnValue;
            }
        }
    }
}
