using System;
using System.Linq;
using System.Xml.Linq;
using BananaXmlOffset.XmlElements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BananaTest.Tests.XmlOffsets.XmlElements
{
    [TestClass]
    public class OffsetElementTest : XElementWrapperTestBase
    {
        [TestMethod]
        public void DefaultConstructor_Name_IsEmpty()
        {
            OffsetElement offset = new OffsetElement();

            Assert.AreEqual("", offset.Name);
        }

        [TestMethod]
        public void Name_Get_ReadsNameElement()
        {
            string expected = "Hi!";
            XElement element = CreateOffsetXElement(expected);
            OffsetElement offset = new OffsetElement(element);

            Assert.AreEqual(expected, offset.Name);
        }

        [TestMethod]
        public void Name_Set_SetsNameElement()
        {
            string expected = "Hi!";

            OffsetElement offset = new OffsetElement
            {
                Name = expected
            };

            string actual = offset.Element.Element("Name").Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RootOperation_Get_NoOperationsElementAndNoNamedPattern_ReturnNull()
        {
            XElement element = CreateOffsetXElement("OffsetName");
            XElement root = CreateRootXElement(element);
            OffsetElement offset = new OffsetElement(element);

            var operation = offset.RootOperation;

            Assert.IsNull(operation);
        }

        [TestMethod]
        public void RootOperation_Get_NoOperationsElement_ReturnNamedPatternOperation()
        {
            XElement element = CreateOffsetXElement("OffsetName");
            XElement pattern = CreatePatternXElement("OffsetName");
            XElement root = CreateRootXElement(pattern, element);
            OffsetElement offset = new OffsetElement(element);

            var operation = offset.RootOperation;
            var patternResult = operation as PatternResult;
            PatternResult_Accessor accessor = new PatternResult_Accessor(new PrivateObject(patternResult));
            Assert.IsNotNull(operation);
            Assert.AreEqual("OffsetName", patternResult.Name);
            Assert.AreEqual(pattern, accessor.PatternElement.Element);
        }

        [TestMethod]
        public void CreateImplicitPatternResult_NoMatchingPattern_ReturnsNull()
        {
            XElement pat1 = CreatePatternXElement("wrong");
            XElement pat2 = CreatePatternXElement("wrongToo");
            XElement element = CreateOffsetXElement("OffsetName");
            XElement root = CreateRootXElement(pat1, pat2, element);

            OffsetElement_Accessor accessor = new OffsetElement_Accessor(element);

            var patternResult = accessor.CreateImplicitPatternResult();

            Assert.IsNull(patternResult);
        }

        [TestMethod]
        public void CreateImplicitPatternResult_MultiplePatterns_UsesPatternWithSameName()
        {
            XElement pat1 = CreatePatternXElement("wrong");
            XElement pat2 = CreatePatternXElement("wrongToo");
            XElement element = CreateOffsetXElement("OffsetName");
            XElement pattern = CreatePatternXElement("OffsetName");
            XElement root = CreateRootXElement(pat1, pat2, pattern, element);

            OffsetElement_Accessor accessor = new OffsetElement_Accessor(element);

            var patternResult = accessor.CreateImplicitPatternResult();

            Assert.AreEqual(pattern, patternResult.PatternElement.Element);
        }

        [TestMethod]
        public void RootOperation_Set_SetsOperationsElement()
        {
            string expected = "C";
            OffsetElement offset = new OffsetElement
            {
                RootOperation = new ConstantResult { Value = expected }
            };

            XElement constantResult = offset.Element.Element("Operations").Element("ConstantResult");

            Assert.AreEqual(expected, constantResult.Value);
        }

        [TestMethod]
        public void RootOperation_Set_ReplacesInnerResultElement()
        {
            string expected = "C";
            XElement innerResult = CreateConstantResultXElement("45");
            XElement operationsElement = new XElement("Operations", innerResult);
            XElement element = CreateOffsetXElement("OffsetName", operationsElement);
            OffsetElement offset = new OffsetElement(element);
            offset.RootOperation = new ConstantResult { Value = expected };

            XElement constantResult = offset.Element.Element("Operations").Element("ConstantResult");

            Assert.AreEqual(expected, constantResult.Value);
            Assert.IsNull(innerResult.Parent);
        }

        [TestMethod]
        public void RootOperation_SetNull_RemovesOperationsElement()
        {
            XElement operations = CreateOperationsXElementWithConstantResult();
            XElement element = CreateOffsetXElement("OffsetName", operations);
            OffsetElement offset = new OffsetElement(element);

            offset.RootOperation = null;

            Assert.IsFalse(element.Elements("Operations").Any());
        }
    }
}
